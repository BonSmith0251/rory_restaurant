#!/usr/bin/env python3
"""RRModKit launcher - see README.md.  Run without arguments for help."""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from rrmodkit.cli import main  # noqa: E402

if __name__ == "__main__":
    sys.exit(main())
