import asyncio
import signal
import uvicorn
from fastapi import FastAPI
import importlib
import os

app = FastAPI()

@app.get("/")
async def read_root():
    return {"Hello": "World"}

def register_routers(app):
    folder_name = "aigc_implements"
    router_files = [f for f in os.listdir(folder_name) if f.endswith(".py")]

    for router_file in router_files:
        module_name = f"{folder_name}.{router_file.rpartition('.')[0]}"
        module = importlib.import_module(module_name)
        app.include_router(module.router)

async def shutdown():
    tasks = [t for t in asyncio.all_tasks() if t is not asyncio.current_task()]
    [task.cancel() for task in tasks]
    await asyncio.gather(*tasks, return_exceptions=True)

def shutdown_server(signum, frame):
    print(f"Received signal {signum}, shutting down server.")
    loop = asyncio.get_event_loop()
    loop.create_task(shutdown())

if __name__ == "__main__":
    register_routers(app)

    loop = asyncio.get_event_loop()
    signal.signal(signal.SIGINT, lambda signum, frame: shutdown_server(signum, frame))
    signal.signal(signal.SIGTERM, lambda signum, frame: shutdown_server(signum, frame))
    uvicorn.run(app, host="0.0.0.0", port=8000)
