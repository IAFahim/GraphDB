import re
import sys
from pathlib import Path

STRUCT_PATTERN = re.compile(r'(public\s+struct\s+)(\w+)(.*?{)', re.DOTALL)
INTERFACE_PATTERN = re.compile(r'IEquatable<\w+>')

EQUALITY_TEMPLATE = """
    public bool Equals({name} other) => Id == other.Id;
    public override bool Equals(object obj) => obj is {name} other && Equals(other);
    public override int GetHashCode() => Id;
"""

def process_struct(match: re.Match) -> str:
    before, name, after = match.groups()

    # If already has IEquatable<T>, skip
    if INTERFACE_PATTERN.search(after):
        return match.group(0)

    # Inject IEquatable<T> into struct declaration
    after_decl = after.replace("{", f", IEquatable<{name}> {{", 1)

    return f"{before}{name}{after_decl}"

def add_interfaces_and_methods(code: str) -> str:
    # Step 1: Add IEquatable<T> to all structs
    code = STRUCT_PATTERN.sub(process_struct, code)

    # Step 2: Insert equality members before closing brace of each struct
    def add_equals_methods(match: re.Match) -> str:
        block = match.group(0)
        struct_name = match.group(1)

        if "bool Equals(" in block:  # already has methods
            return block

        # Inject before last closing brace
        insert_pos = block.rfind("}")
        return block[:insert_pos] + EQUALITY_TEMPLATE.format(name=struct_name) + block[insert_pos:]

    # Match entire struct bodies
    struct_body_pattern = re.compile(
        r'(?:public\s+struct\s+)(\w+).*?\{(.*?)\n\}', re.DOTALL
    )

    code = re.sub(
        r'(public\s+struct\s+)(\w+)(.*?\{(?:[^{}]*|\{[^{}]*\})*\})',
        lambda m: insert_equals(m),
        code,
        flags=re.DOTALL
    )

    return code


def insert_equals(match: re.Match) -> str:
    prefix, name, body = match.groups()

    # Already has Equals?
    if f"Equals({name}" in body:
        return match.group(0)

    # Insert before last closing brace
    insert_pos = body.rfind("}")
    new_body = body[:insert_pos] + EQUALITY_TEMPLATE.format(name=name) + body[insert_pos:]
    return f"{prefix}{name}{new_body}"


def main(filepath: str):
    path = Path(filepath)
    code = path.read_text(encoding="utf-8")

    updated = add_interfaces_and_methods(code)

    backup_path = path.with_suffix(".bak.cs")
    path.rename(backup_path)

    path.write_text(updated, encoding="utf-8")
    print(f"✅ Updated file written to {path}, backup saved as {backup_path}")


if __name__ == "__main__":
    main("GameDatabase.cs")
