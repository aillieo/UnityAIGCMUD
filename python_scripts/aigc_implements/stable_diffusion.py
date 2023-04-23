from fastapi import APIRouter, Request, HTTPException

router = APIRouter()

@router.post("/sd")
async def print_request_data(request: Request):
    content_type = request.headers.get("Content-Type")
    user_agent = request.headers.get("User-Agent")
    if content_type != "application/json" :
        raise HTTPException(status_code=400, detail="Invalid request data")

    data = await request.json()

    prompt = data.get("prompt")

    if not isinstance(prompt, str):
        raise HTTPException(status_code=400, detail="Invalid request data")

    return {"url": "url/of/some/image"}

@router.get("/sd/images/{image_id}")
async def read_item(image_id: int):
    return {"url": "url/of/some/image"}