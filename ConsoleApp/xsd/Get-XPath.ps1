# https://www.codewise-llc.com/blog/2015/1/19/running-xsl-transforms-whenever-you-change-your-xsl-file
# https://stackoverflow.com/questions/10636805/get-current-nodes-xpath
param (
    $xmlFile = '..\..\Data\20180901-gleif-concatenated-file-lei2-Top1000.xml',
    $xslFile = '.\Get-XPath.xslt',
    $outputFile = $xmlFile.Replace('.xml', '.xpath-posh.txt')
)
Try
{
    # Create transform Arguments
    $XmlUrlResolver = New-Object System.Xml.XmlUrlResolver;
    $xslt_settings = New-Object System.Xml.Xsl.XsltSettings;
    $xslt_settings.EnableScript = 1;

    # Create the transform
    $xslt = New-Object System.Xml.Xsl.XslCompiledTransform;
    $xslt.Load($xslFile, $xslt_settings, $XmlUrlResolver);

    # Run the transform
    $xslt.Transform($xmlFile, $outputFile);
}
Catch
{
    $ErrorMessage = $_.Exception.Message
    $FailedItem = $_.Exception.ItemName
    Write-Host  'Error'$ErrorMessage':'$FailedItem':' $_.Exception;
}