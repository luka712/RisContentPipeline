# python
"""
Processor for RisContentPipeline: updates TexturePacker bundle image to .ktx2.
"""

import json
import os
import sys

from base_processor import create_error_result, create_success_result

def convert(source, options) -> dict:
    
    # Nothing to do if there is no source or file path.
    if source is None or source.file_path is None:
        return create_success_result()

    if options is None or options.output_path is None:
        return create_error_result("Output path is not specified in options.")

    # We take in json as a string, so we need to parse it first.
    json_str = api.read_json_as_str(source.file_path)
    data = json.loads(json_str)

    # TexturePacker has 'meta' property. See if 'app' is correct.
    meta = data.get("meta", {})

    if meta.get("app") == "https://www.codeandweb.com/texturepacker":
         # change 'image' property to end with '.ktx2'.
        image = meta.get("image", "")
        if not image:
            return create_success_result()
        meta["image"] = os.path.splitext(image)[0] + ".ktx2"

        json_str = json.dumps(data, indent=4)
        api.save_json(options.output_path, json_str)

    return create_success_result()
    
def before_build():
    
   if api.has_pipeline("texture_packer_json"):
      return
    
   # Add pipeline that will convert json to any other json type.
   api.add_pipeline("texture_packer_json", ["json"], ["*", "json"], convert )