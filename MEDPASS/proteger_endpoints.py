from pathlib import Path
import re

controllers_path = Path("Controllers")

for file in controllers_path.glob("*.cs"):
    if file.name == "AuthController.cs":
        print(f"Saltando {file.name}")
        continue

    text = file.read_text()

    # Agregar using de Authorization si no existe
    if "using Microsoft.AspNetCore.Authorization;" not in text:
        text = text.replace(
            "using Microsoft.AspNetCore.Mvc;",
            "using Microsoft.AspNetCore.Mvc;\nusing Microsoft.AspNetCore.Authorization;"
        )

    # Proteger el controlador completo para Medico y Paciente
    # Esto hace que todos los métodos pidan token.
    if '[Authorize(Roles = "Medico,Paciente")]' not in text:
        text = re.sub(
            r'(\[ApiController\]\s*)',
            r'\1[Authorize(Roles = "Medico,Paciente")]\n',
            text,
            count=1
        )

    lines = text.splitlines()
    new_lines = []

    for i, line in enumerate(lines):
        stripped = line.strip()

        # Si encuentra métodos de modificación, agrega Authorize Medico
        if stripped.startswith("[HttpPost") or stripped.startswith("[HttpPut") or stripped.startswith("[HttpPatch") or stripped.startswith("[HttpDelete"):
            previous = "\n".join(new_lines[-3:])
            if '[Authorize(Roles = "Medico")]' not in previous:
                indent = line[:len(line) - len(line.lstrip())]
                new_lines.append(indent + '[Authorize(Roles = "Medico")]')

        new_lines.append(line)

    file.write_text("\n".join(new_lines) + "\n")
    print(f"Protegido: {file.name}")
