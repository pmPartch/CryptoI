# Cryptography I Notes

## a Coursera Course presented by Dan Boneh of Stanford University

Here is an example that you can work out by hand if you wish. Most of the digits are in hex (except where noted).
I like to start learning a topic such as this one with having all the information about the cleartext, key, and ciphertext. Note that for the real problem, you only know the ciphertext and that it is an 128 bit AES using CBC. But having all of this additional information might be useful in learning the technique as well as debugging an algorithm that will be used on the actual problem.

key: BCD494EDCD65A5B9F8F3033678627362

ciphertext: 9D67966D3707CA44E0F90F7AF23E24D9D18DEDD46FC4D249F932BEA3949AB1542D4F8C3FDA00C01167D3B9D39EFB879E2A2B68BBF01F4FAE3FF08D9F1FC73CE8

cleartext: My simple bit of plain text to encrypt.

Note: For this note, digits are in decimal. This is 128 bit AES CBC with padding (all padding bytes have the same digit: the number of padding bytes).
      There is always padding. The padding number is between [1,16] inclusive (ie, you cant have a padding byte of 0 or 17,18, or greater.
      The ciphertext is always an even multiple of 16 bytes (128 bites). The first 16 bytes is the IV. Lets refer to the 16 bytes as a 'block'.
      So this ciphertext is 4 blocks long (includes the IV). The last block contains 9 padding bytes for this specific plaintext encryption.

Here are the intermediate results of a decryption (these are XOR'ed with the IV or the cipher text):

IV: 9D67966D3707CA44E0F90F7AF23E24D9  

c0: D18DEDD46FC4D249F932BEA3949AB154  
c1: 2D4F8C3FDA00C01167D3B9D39EFB879E  
c2: 2A2B68BBF01F4FAE3FF08D9F1FC73CE8  

D(k.c0) : D01EB61E5E6ABA2885D96D13861E4BBF  
D(k.c1) : F1FD81B506AAF23D9C4ACA83E0F59131  
D(k.c2) : 432CFE56AA74EE186EDAB0DA97F28E97  

So,

IV XOR D(k,c0) = 4D792073696D706C6520626974206F66  

c0 XOR D(k,c1) = 20706C61696E207465787420746F2065  

c1 XOR D(k,c2) = 6E63726970742E090909090909090909  (yes, the padding is 9 bytes long)  


Decoding the ASCII in the above three lines gives:

m0 = IV XOR D(k,c0) = 'My simple bit of'  

m1 = c0 XOR D(k,c1) = ' plain text to e'  

m2 = c1 XOR D(k,c2) = 'ncript.'  (after removing the padding)


Now, from the picture shown below, lets say your 'lucky' guess g = 65 for the last char of m1 , so lets replace the last byte of c0 with
54 XOR 65 XOR 01 = 30

now note that if we take this result and XOR  the last byte of the output of D(k,c1) we get:
31 XOR 30 = 01 (a padding value of 01 is valid. A one byte pad)

Now continuing to the next byte to deduce the m1 byte 20 (a space in ASCII) is to change the padding to 0202 (two byte pad)

![alt text](https://github.com/pmPartch/CryptoI/raw/master/CBC_decode.PNG "AES with CBC")
