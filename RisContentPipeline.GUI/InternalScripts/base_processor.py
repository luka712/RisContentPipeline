"""
Generic Python processor for RisContentPipeline.

This module provides a flexible input/output processing system that can be
extended to handle various content pipeline operations.
"""
from typing import Any, Dict


def create_success_result() -> dict:
    """
        Create a successful result.
    :return:
        A dictionary representing the successful.
    """
    return {
        "success": True,
    }

def create_error_result(message: str) -> dict:
    """
    Create an error result.
    Args:
        message: The error message.
    Returns:
        A dictionary representing the error result.
    """
    return {
        "success": False,
        "error": message
    }

def process_asset() -> dict:
    """
    Called when specific asset is being processed. Asset can be accessed with 'api.current_asset'.
        
    Returns:
        A dictionary representing the result of processing the file.
        Use json_result(data) for successful processing and error(message) for any errors encountered.
    """
    return create_success_result()
