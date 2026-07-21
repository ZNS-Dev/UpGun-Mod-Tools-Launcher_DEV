#!/bin/bash

read -p "Entrez le numéro de version (ex: 1.0.1) : " VERSION

if [ -z "$VERSION" ]; then
    echo "[ERREUR] Aucune version renseignée."
    exit 1
fi

# Nettoie le 'v' si déjà saisi (ex: "v1.0.1" -> "1.0.1")
VERSION="${VERSION#v}"

echo -e "\n[1/2] Création du tag v$VERSION..."
git tag "v$VERSION" || { echo "[ERREUR] Échec de la création du tag."; exit 1; }

echo -e "\n[2/2] Envoi du tag vers origin..."
git push origin "v$VERSION" || { echo "[ERREUR] Échec du push."; exit 1; }

echo -e "\n[SUCCÈS] Le tag v$VERSION a été poussé !"