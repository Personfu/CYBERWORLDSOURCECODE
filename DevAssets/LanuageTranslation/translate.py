import os
import sys
import json
import torch
from transformers import MarianMTModel, MarianTokenizer
from tqdm import tqdm

os.environ["HF_HUB_DISABLE_SYMLINKS_WARNING"] = "1"

if os.name == "nt":
    try:
        sys.stdout.reconfigure(encoding="utf-8")
    except Exception:
        pass

os.chdir(os.path.dirname(os.path.abspath(__file__)))

SOURCE_FILE = "en_US.json.json"

DEFAULT_BATCH_SIZE_CPU = 32
DEFAULT_BATCH_SIZE_GPU = 32

USE_FP16_ON_GPU = True

languages = {
    "es_LA": "es",
    "fr_FR": "fr",
    "pt_BR": "pt"
}

hf_model_map = {
    "es": "Helsinki-NLP/opus-mt-en-es",
    "fr": "Helsinki-NLP/opus-mt-en-fr",
    "pt": "Helsinki-NLP/opus-mt-tc-big-en-pt"
}

BASE_DIR = os.getcwd()
MODEL_DIR = os.path.join(BASE_DIR, "models")
os.makedirs(MODEL_DIR, exist_ok=True)

if torch.cuda.is_available():
    device = "cuda"
    gpu_available = True
elif getattr(torch.version, "hip", None) is not None:
    device = "cuda"
    gpu_available = True
else:
    device = "cpu"
    gpu_available = False

env_batch = os.environ.get("TRANSLATE_BATCH_SIZE")
if env_batch:
    try:
        BATCH_SIZE = int(env_batch)
    except Exception:
        BATCH_SIZE = DEFAULT_BATCH_SIZE_GPU if gpu_available else DEFAULT_BATCH_SIZE_CPU
else:
    BATCH_SIZE = DEFAULT_BATCH_SIZE_GPU if gpu_available else DEFAULT_BATCH_SIZE_CPU

print("Using device:", device)
print("GPU available:", gpu_available)
print("Batch size:", BATCH_SIZE)
print()

def download_or_update_models():
    """
    Ensure all translation models and tokenizers are present locally (cached).
    We set force_download to False so existing cache is reused.
    """
    for code, model_name in hf_model_map.items():
        path = os.path.join(MODEL_DIR, code)
        print(f"Downloading or updating model for {code}...")
        MarianMTModel.from_pretrained(model_name, cache_dir=path, force_download=False)
        MarianTokenizer.from_pretrained(model_name, cache_dir=path, force_download=False)
    print("All models ready.\n")

def translate_batch(batch, model, tokenizer, device):
    results = {}
    texts = [v for _, v in batch]
    keys = [k for k, _ in batch]
    encoded = tokenizer(texts, return_tensors="pt", truncation=True, padding=True)
    encoded = {k: v.to(device) for k, v in encoded.items()}
    with torch.no_grad():
        output = model.generate(**encoded)
    decoded = tokenizer.batch_decode(output, skip_special_tokens=True)
    for k, t in zip(keys, decoded):
        results[k] = t
    return results

def translate_language(data, lang_code, model_code, device):
    model_name = hf_model_map[model_code]
    path = os.path.join(MODEL_DIR, model_code)

    print(f"Loading model for {lang_code}...")
    tokenizer = MarianTokenizer.from_pretrained(model_name, cache_dir=path)
    model = MarianMTModel.from_pretrained(model_name, cache_dir=path)

    model.to(device)
    if device != "cpu" and USE_FP16_ON_GPU:
        try:
            model.half()
            print("Model converted to FP16 (half precision).")
        except Exception:
            print("FP16 conversion failed or not supported; continuing with full precision.")

    model.eval()

    items = list(data.items())
    batches = [items[i:i+BATCH_SIZE] for i in range(0, len(items), BATCH_SIZE)]
    output_data = {}

    for batch in tqdm(batches, desc=f"Translating {lang_code}", ncols=100):
        result = translate_batch(batch, model, tokenizer, device)
        output_data.update(result)
        if device != "cpu":
            try:
                torch.cuda.empty_cache()
            except Exception:
                pass

    out_path = os.path.join(BASE_DIR, f"{lang_code}.json.json")
    with open(out_path, "w", encoding="utf-8") as f:
        json.dump(output_data, f, ensure_ascii=False, indent=4)
    print(f"{lang_code} -> {out_path}\n")

def main():
    download_or_update_models()

    en_path = os.path.join(BASE_DIR, SOURCE_FILE)
    if not os.path.exists(en_path):
        print("Missing file:", en_path)
        return

    with open(en_path, "r", encoding="utf-8") as f:
        try:
            data = json.load(f)
        except Exception as e:
            print("Failed to parse JSON source file:", e)
            return

    for lang, code in languages.items():
        translate_language(data, lang, code, device)

    print("All translations completed.")

if __name__ == "__main__":
    main()
    input("\nPress Enter to close...")
