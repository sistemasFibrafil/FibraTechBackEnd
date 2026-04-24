$path = 'c:\Users\jhuaman\Documents\PROYECTO_FIL_NUEVO\FibraTechBackEnd\Net.Data\SAPBusinessOne\BusinessPartners\BusinessPartners\BusinessPartnersRepository.cs'
$content = Get-Content $path
$newContent = New-Object System.Collections.Generic.List[string]

foreach ($line in $content) {
    if ($line -match 'U_BPP_BPTD' -or $line -match 'U_BPP_BPTP') {
        continue
    }
    # Fix casing for Email UDFs in SAP Item call
    if ($line -match 'bp.UserFields.Fields.Item\("U_FIB_Email2"\)') {
        $line = $line -replace '"U_FIB_Email2"', '"U_FIB_EMAIL2"'
    }
    if ($line -match 'bp.UserFields.Fields.Item\("U_FIB_Email3"\)') {
        $line = $line -replace '"U_FIB_Email3"', '"U_FIB_EMAIL3"'
    }
    $newContent.Add($line)
}
$newContent | Set-Content $path
