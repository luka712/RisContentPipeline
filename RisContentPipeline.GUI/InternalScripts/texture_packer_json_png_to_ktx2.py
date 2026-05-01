# python
"""
Processor for RisContentPipeline: updates TexturePacker bundle image to .ktx2.
"""

import json
import os
import sys

from base_processor import create_error_result, create_success_result


def process_asset() -> dict:

    asset = api.get_current_asset()

    if asset is None:
        return create_error_result("No current asset found.")

    if not asset.is_json:
        return create_success_result()

    data = json.loads(asset.content)

    # TexturePacker has 'meta' property. See if 'app' is correct.
    meta = data.get("meta", {})
    if meta.get("app") != "https://www.codeandweb.com/texturepacker":
        return create_success_result()

    # change 'image' property to end with '.ktx2'.
    image = meta.get("image", "")
    if not image:
        return create_success_result()
    meta["image"] = os.path.splitext(image)[0] + ".ktx2"

    asset.content = json.dumps(data, indent=4)

    return create_success_result()