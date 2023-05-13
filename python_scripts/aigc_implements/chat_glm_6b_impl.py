from fastapi import APIRouter, Request, HTTPException

router = APIRouter()

@router.post("/glm")
async def chat(request: Request):
    content_type = request.headers.get("Content-Type")
    user_agent = request.headers.get("User-Agent")
    if content_type != "application/json" :
        raise HTTPException(status_code=400, detail="Invalid request data")

    data = await request.json()

    prompt = data.get("prompt")
    history = data.get("history")

    print(f"Received request: {data}")

    if not isinstance(prompt, str) or not isinstance(history, list):
        raise HTTPException(status_code=400, detail="Invalid request data")

    history.append(prompt)
    history_text = "\n".join(history)

    print(f"History: {history_text}")

    return {"text": history_text}
