# python
"""
Processor for RisContentPipeline: updates TexturePacker bundle image to .ktx2.
"""

import json
import os
import sys

from base_processor import create_ignore_result, create_error_result, create_json_result


def process_file(file_path: str) -> dict:

    # Ignore non json files.
    if not file_path.endswith(".json"):
        return create_ignore_result()

    ## try to load json.
    try:
        with open(file_path, "r", encoding="utf-8") as f:
            data = json.load(f)
    except FileNotFoundError:
        return create_error_result(f"File not found: {file_path}")
    except json.JSONDecodeError as e:
        return create_error_result(f"JSON decode error: {e}")

    # TexturePacker has 'meta' property. See if 'app' is correct.
    meta = data.get("meta", {})
    if meta.get("app") != "https://www.codeandweb.com/texturepacker":
        return create_ignore_result()

    # change 'image' property to end with '.ktx2'.
    image = meta.get("image", "")
    if not image:
        return create_ignore_result()
    meta["image"] = os.path.splitext(image)[0] + ".ktx2"

    return create_json_result(data)


def main():
    file_path = sys.argv[1] if len(sys.argv) > 1 else "IconsBundle0.json"
    result = process_file(file_path)
    if not result["success"]:
        print(f"Error processing file: {result['error']}")
        sys.exit(1)
    print("File processed successfully.")


if __name__ == "__main__":
    main()
