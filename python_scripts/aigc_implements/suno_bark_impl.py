from fastapi import APIRouter, Request, HTTPException
import numpy as np
from bark import SAMPLE_RATE, generate_audio, preload_models
import scipy.io.wavfile as wavfile
import io
import base64

_ = preload_models()

router = APIRouter()

@router.post("/bark")
async def tts(request: Request):
    content_type = request.headers.get("Content-Type")
    user_agent = request.headers.get("User-Agent")
    if content_type != "application/json" :
        raise HTTPException(status_code=400, detail="Invalid request data")

    data = await request.json()

    prompt = data.get("prompt")

    if not isinstance(prompt, str):
        raise HTTPException(status_code=400, detail="Invalid request data")

    print(prompt)

    audio_arr = generate_audio(prompt, history_prompt='announcer')
    audio_arr = (audio_arr * 32767).astype(np.int16)

    with io.BytesIO() as wav_bytes:
        wavfile.write(wav_bytes, SAMPLE_RATE, audio_arr)
        wav_bytes.seek(0)
        wav_bytes_io = wav_bytes.read()
    base64_str = base64.b64encode(wav_bytes_io).decode('utf-8')
    return {"prompt": prompt, "audio" : base64_str}
