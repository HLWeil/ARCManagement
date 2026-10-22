"""Public Python facade for the shared F# library."""

from fable_library.async_ import start_as_task

from .py.example_model import ExampleModel
from .py.runtime import get_runtime_name
from .py.file_io import (
    ensure_directory_async as _ensure_directory_async,
    read_text_async as _read_text_async,
    write_text_async as _write_text_async,
)

__all__ = [
    "ExampleModel",
    "get_runtime_name",
    "ensure_directory_async",
    "read_text_async",
    "write_text_async",
]


async def ensure_directory_async(path: str) -> None:
    await start_as_task(_ensure_directory_async(path))


async def read_text_async(path: str) -> str:
    return await start_as_task(_read_text_async(path))


async def write_text_async(path: str, text: str) -> None:
    await start_as_task(_write_text_async(path, text))
