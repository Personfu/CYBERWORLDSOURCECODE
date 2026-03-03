@echo off
cd /d "%~dp0"

echo Checking Python installation...
python --version >nul 2>&1
IF ERRORLEVEL 1 (
    echo Python is not installed or not in PATH.
    echo Please install Python 3.10+ from https://www.python.org/
    pause
    exit /b
)

echo Updating pip...
python -m pip install --upgrade pip

echo Checking and installing required packages...

pip show torch >nul 2>&1 || pip install torch --index-url https://download.pytorch.org/whl/cpu

pip show transformers >nul 2>&1 || pip install transformers
pip show tqdm >nul 2>&1 || pip install tqdm
pip show sentencepiece >nul 2>&1 || pip install sentencepiece
pip show sacremoses >nul 2>&1 || pip install sacremoses
pip show protobuf >nul 2>&1 || pip install protobuf
pip show safetensors >nul 2>&1 || pip install safetensors
pip show huggingface_hub >nul 2>&1 || pip install huggingface_hub
pip show hf-xet >nul 2>&1 || pip install hf-xet

echo Running translation script...
python translate.py

echo.
pause