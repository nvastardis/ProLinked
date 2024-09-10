# Delete Bin and OBJ folders in Project folders of the ASP solution

# Set the path to the root directory of your solution
$solutionRoot = "..\aspnet-core"

# Get all bin and obj folders recursively within the solution directory
$foldersToDelete = Get-ChildItem -Path $solutionRoot -Recurse -Directory -Include "bin", "obj"

# Loop through each folder and delete it
foreach ($folder in $foldersToDelete) {

    # Display the folder path in yellow color
    Write-Host "Deleting folder: $($folder.FullName)..." -ForegroundColor Yellow

    Remove-Item -Path $folder.FullName -Recurse -Force
}

Write-Output "Deleted all bin and obj folders within the solution directory."