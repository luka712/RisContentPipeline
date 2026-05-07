# python
"""
Processor for RisContentPipeline: updates TexturePacker bundle image to .ktx2.
"""

import json
import os
import sys

from base_processor import create_error_result, create_success_result

def convert(json, options) -> dict:
    
    return {
        "success": True
    }
    

def before_build():
   api.create_pipeline("texture_packer_json", ["json"], ["json"], convert )