#!/bin/bash

# Open a new terminal window and run everything inside
gnome-terminal -- bash -c "
# Name of virtual environment
VENV_DIR='venv'

# Create virtual environment if it doesn't exist
if [ ! -d \"\$VENV_DIR\" ]; then
    python3 -m venv \"\$VENV_DIR\"
fi

# Activate virtual environment
source \"\$VENV_DIR/bin/activate\"

# Upgrade pip
pip install --upgrade pip

# Install required packages
# PyTorch needs the CPU index URL
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cpu
pip install transformers sentencepiece tqdm protobuf

# Run translation script
python3 translate.py

# Keep terminal open
echo ''
echo 'Translation finished. Press Enter to close...'
read
"
