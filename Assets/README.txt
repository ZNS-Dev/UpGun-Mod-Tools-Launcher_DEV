Dépose ici ton fichier ModLauncher.ico (le même que dans le projet WinForms
d'origine) pour qu'il soit utilisé comme icône d'application.

Le .csproj le référence de façon conditionnelle (Condition="Exists(...)"),
donc le build ne casse pas si le fichier est absent — l'appli utilisera
simplement l'icône par défaut d'Avalonia en attendant.
