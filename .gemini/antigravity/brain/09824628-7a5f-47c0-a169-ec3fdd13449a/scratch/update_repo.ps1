$path = 'c:\Users\jhuaman\Documents\PROYECTO_FIL_NUEVO\FibraTechBackEnd\Net.Data\SAPBusinessOne\BusinessPartners\BusinessPartners\BusinessPartnersRepository.cs'
$content = Get-Content $path
$newContent = New-Object System.Collections.Generic.List[string]

$email3FoundCount = 0
foreach ($line in $content) {
    $newContent.Add($line)
    if ($line -match 'U_FIB_Email3') {
        $email3FoundCount++
        if ($email3FoundCount -eq 2) {
            $newContent.Add('                    if (!string.IsNullOrEmpty(value.U_FIB_Transp)) bp.UserFields.Fields.Item("U_FIB_Transp").Value = value.U_FIB_Transp;')
            $newContent.Add('                    if (!string.IsNullOrEmpty(value.U_FIB_Creed)) bp.UserFields.Fields.Item("U_FIB_Creed").Value = value.U_FIB_Creed;')
        }
    }
}
$newContent | Set-Content $path
