Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [System.IO.Compression.ZipFile]::OpenRead("c:\Users\rgtt0\Downloads\Merge_Idle_Puzzle_Game_GDD.docx")
$entry = $zip.GetEntry("word/document.xml")
$stream = $entry.Open()
$reader = New-Object System.IO.StreamReader($stream)
$content = $reader.ReadToEnd()
$reader.Close()
$zip.Dispose()
$text = $content -replace '<w:p [^>]*>', "`n" -replace '<[^>]+>', ' ' -replace '&amp;', '&' -replace '&lt;', '<' -replace '&gt;', '>' -replace '\s+', ' '
[System.IO.File]::WriteAllText("d:\Project\MergIdel\gdd_extract.txt", $text, [System.Text.Encoding]::UTF8)
