# EZDump

EZDump is a simple C# program that creates a mini dump of the Local Security Authority Subsystem 
Service (LSASS) process memory in Windows systems. The memory dump is saved as a base64 encoded 
string in a file called `out.txt`, which can be further analyzed using tools such as Pypykatz or 
Mimikatz's sekurlsa::minidump.

The code has been tested on Windows 10 and may not be compatible with other Windows operating 
systems.

# Usage

To use EZDump, simply run the tool **as an administrator** without any arguments. This will 
automatically locate the LSASS PID and create a base64 encoded minidump, saved as `out.txt` in your 
working directory.

Once you have the `out.txt` file, you can decode the minidump on MacOS using the following command:

`cat out.txt | base64 -D > out.dmp`

You can then analyze the minidump using PypyKatz with the following command:

`pypykatz lsa minidump out.dmp`

# Troubleshooting
If the program fails to create a dump, it will output an error message along with the last Win32 
error code or exception details. Please ensure that you are running the program with administrative 
privileges and that there are no issues with accessing the LSASS process.

Note that certain security features such as RunAsPPL (Protected Process Light) and Credential Guard, 
when enabled, will prevent unauthorized access to the LSASS process and may block this tool from 
functioning.
