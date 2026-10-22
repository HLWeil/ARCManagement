import asyncio

import pytest

from polyglot_template import (
    ensure_directory_async,
    read_text_async,
    write_text_async,
)


def test_text_files(tmp_path):
    async def check():
        nested = tmp_path / "nested" / "child"
        await ensure_directory_async(str(nested))
        path = str(nested / "example.txt")
        text = "Hello, 世界!\nGrüße 👋\n"
        await write_text_async(path, text)
        assert await read_text_async(path) == text
        await write_text_async(path, "replacement")
        assert await read_text_async(path) == "replacement"
        await write_text_async(path, "")
        assert await read_text_async(path) == ""
        with pytest.raises(FileNotFoundError):
            await read_text_async(str(tmp_path / "missing.txt"))
        with pytest.raises(FileNotFoundError):
            await write_text_async(str(tmp_path / "missing" / "file.txt"), "text")

    asyncio.run(check())
