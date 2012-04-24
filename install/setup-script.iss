; -- Example1.iss --
; Demonstrates copying 3 files and creating an icon.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=AppHarbor CLI
AppPublisher=AppHarbor, Inc.
AppPublisherURL=https://appharbor.com
AppSupportURL=http://support.appharbor.com
AppUpdatesURL=https://github.com/appharbor/appharbor-cli/downloads
AppVersion=0.1
ChangesEnvironment=yes
Compression=lzma2
DefaultDirName={pf}\AppHarbor
OutputDir=userdocs:AppHarbor CLI setup output
SolidCompression=yes

[Files]
Source: "..\src\AppHarbor\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion; Excludes: "*.xml,*.pdb"

[Registry]
root: HKLM; Subkey: "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; ValueType: expandsz; ValueName: "Path"; ValueData: "{olddata};{app}"; Check: NeedsAddPath('{app}')

[Code]
function NeedsAddPath(Param: string): boolean;
var
  OrigPath: string;
begin
  if not RegQueryStringValue(HKEY_LOCAL_MACHINE,
    'SYSTEM\CurrentControlSet\Control\Session Manager\Environment',
    'Path', OrigPath)
  then begin
    Result := True;
    exit;
  end;
  // look for the path with leading and trailing semicolon
  // Pos() returns 0 if not found
  Result := Pos(';' + Param + ';', ';' + OrigPath + ';') = 0;
end;