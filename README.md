# Presentation

CryptoSoft is a software that can encrypt or decrypt any file using XOR byte per byte technique.

# Syntax

Command line to use : CryptoSoft.exe source [destination] [secret]

Example : CryptoSoft.exe "C:\Users\user\Desktop\test.txt" "C:\Users\user\Desktop\test.txt.enc" edOdDYC0amtMJhlzyiGbNiOeLMXksBMj

# Important Informations

The Crypto Key must be 64 bytes long minimum.

# Return code of the exe

-1 : Error while processing encryption.

-2 : File is ignored by user settings.

> 0 : The value returned is the encryption time of the file in milliseconds.
