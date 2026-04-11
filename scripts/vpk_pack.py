#!/usr/bin/env python3
"""
vpk_pack - Source 2 VPK v2 archive creator
Usage: python vpk_pack.py <folder> <output>
"""

import os
import sys
import struct
import hashlib
import binascii

VPK_SIGNATURE = 0x55AA1234
VPK_VERSION   = 2
VPK_EMBEDDED  = 0x7FFF
VPK_TERMINATOR = 0xFFFF


def collect_files(folder):
    """Collect all files recursively, returning a list of (rel_path, data) tuples."""
    entries = []
    for dirpath, _, filenames in os.walk(folder):
        for filename in filenames:
            abs_path = os.path.join(dirpath, filename)
            rel_path = os.path.relpath(abs_path, folder).replace("\\", "/")
            with open(abs_path, "rb") as f:
                data = f.read()
            entries.append((rel_path, data))
    return entries


def split_path(rel_path):
    """Split rel_path into (ext, dir, name) matching VPK conventions."""
    parts = rel_path.rsplit("/", 1)
    directory = parts[0] if len(parts) == 2 else " "
    filename  = parts[-1]

    dot = filename.rfind(".")
    if dot != -1:
        name = filename[:dot]
        ext  = filename[dot + 1:]
    else:
        name = filename
        ext  = " "

    return ext, directory, name


def build_string(s):
    """Null-terminated string as bytes."""
    return s.encode("utf-8") + b"\x00"


def build_vpk(entries):
    """
    Build the VPK v2 binary content.
    Returns the complete file bytes.
    """
    # Group files by ext -> dir -> list of (name, data, crc)
    tree = {}
    for rel_path, data in entries:
        ext, directory, name = split_path(rel_path)
        crc = binascii.crc32(data) & 0xFFFFFFFF
        tree.setdefault(ext, {}).setdefault(directory, []).append((name, data, crc))

    tree_buf    = bytearray()
    filedata_buf = bytearray()

    for ext, dirs in tree.items():
        tree_buf += build_string(ext)
        for directory, files in dirs.items():
            tree_buf += build_string(directory)
            for name, data, crc in files:
                tree_buf += build_string(name)

                data_offset = len(filedata_buf)
                filedata_buf += data

                # DirectoryEntry: crc, preload_bytes, archive_index, entry_offset, entry_length, terminator
                tree_buf += struct.pack("<IHHIIH",
                    crc,
                    0,           # PreloadBytes
                    VPK_EMBEDDED,
                    data_offset,
                    len(data),
                    VPK_TERMINATOR,
                )

            tree_buf += b"\x00"  # end of filenames for this dir
        tree_buf += b"\x00"      # end of dirs for this ext
    tree_buf += b"\x00"          # end of extensions

    # Header
    header = struct.pack("<IIIIIII",
        VPK_SIGNATURE,
        VPK_VERSION,
        len(tree_buf),       # TreeSize
        len(filedata_buf),   # FileDataSectionSize
        0,                   # ArchiveMD5SectionSize (embedded, no archive chunks)
        48,                  # OtherMD5SectionSize (3 x 16 bytes)
        0,                   # SignatureSectionSize
    )

    # MD5 checksums (OtherMD5Section)
    tree_md5        = hashlib.md5(tree_buf).digest()
    archive_md5_md5 = hashlib.md5(b"").digest()   # ArchiveMD5 section is empty
    whole_md5       = hashlib.md5(header + tree_buf + filedata_buf).digest()

    return header + bytes(tree_buf) + bytes(filedata_buf) + tree_md5 + archive_md5_md5 + whole_md5


def main():
    if len(sys.argv) != 3:
        print("Usage: python VpkCreator.py <folder> <output>")
        print("  Example: python VpkCreator.py assets data  -->  data.vpk")
        sys.exit(1)

    folder = sys.argv[1]
    output = sys.argv[2]

    if not os.path.isdir(folder):
        print(f"Error: '{folder}' is not a valid directory")
        sys.exit(1)

    out_path = output if output.lower().endswith(".vpk") else output + ".vpk"

    entries = collect_files(folder)
    if not entries:
        print(f"Error: no files found in '{folder}'")
        sys.exit(1)

    for rel_path, _ in entries:
        print(f"  + {rel_path}")

    vpk_bytes = build_vpk(entries)

    with open(out_path, "wb") as f:
        f.write(vpk_bytes)

    print(f"Created '{out_path}' with {len(entries)} file(s).")


if __name__ == "__main__":
    main()
