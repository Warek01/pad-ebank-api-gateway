#!/bin/bash

set -e  # Exit immediately if a command exits with a non-zero status

OUTPUT="app/generated"
INPUT="app/proto"

# Clean and create output directory
rm -rf "$OUTPUT"
mkdir -p "$OUTPUT"

# Generate Python code from .proto files
if ! python -m grpc_tools.protoc \
  -I "$INPUT" \
  --python_out="$OUTPUT" \
  --pyi_out="$OUTPUT" \
  --grpc_python_out="$OUTPUT" \
  "$INPUT"/*.proto; then
  echo "Error: Protobuf compilation failed."
  exit 1
fi

# Define the namespace for imports
NAMESPACE="app.generated"
declare -a MODULES=(
  "shared_pb2"
  "account_service_pb2"
  "transaction_service_pb2"
)

# Update import statements in generated files
find "$OUTPUT" -type f \( -name "*.py" -o -name "*.pyi" \) | while read -r file; do
  for module in "${MODULES[@]}"; do
    ORIGINAL_IMPORT="import $module as ${module//_/__}"
    REPLACEMENT_IMPORT="import $NAMESPACE.$module as ${module//_/__}"

    # Use sed to replace the original import with the new import
    sed -i "s|$ORIGINAL_IMPORT|$REPLACEMENT_IMPORT|g" "$file"
  done
done

echo "Import statements updated successfully."
