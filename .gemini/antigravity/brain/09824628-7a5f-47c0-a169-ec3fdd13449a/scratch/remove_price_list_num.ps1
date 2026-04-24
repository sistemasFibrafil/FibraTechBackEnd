$path = 'c:\Users\jhuaman\Documents\PROYECTO_FIL_NUEVO\FibraTechBackEnd\Net.Data\SAPBusinessOne\BusinessPartners\BusinessPartners\BusinessPartnersRepository.cs'
$content = Get-Content $path
$newContent = New-Object System.Collections.Generic.List[string]

foreach ($line in $content) {
    if ($line -notmatch 'PriceListNum') {
        $newContent.Add($line)
    }
}
$newContent | Set-Content $path
