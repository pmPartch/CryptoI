# Cryptography I Notes

## a Coursera Course presented by Dan Boneh of Stanford University

## Padding Oracle Notes

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


Now, from the picture shown below and compare with my starting, intermediate, and final results above.


### step 1

remove the last block (with pad)....actually, my solution to this problem starts with this last block, but lets first start with the easy stuff...

### step 2 : guess the last byte of m1 (this may take 256 attempts)

Start guessing at this last byte of m1 (g = 0x00, g = 0x01, etc) and start doing the algorithm noted in the picture below. I'll show this calculation for the one correct answer below (where the padding oracle is returning a bad address but good pad).

your good guess g = 65 for the last char of m1 , so lets replace the last byte of c0 with
54 XOR 65 XOR 01 = 30. Now note that if we take this result and XOR  the last byte of the output of D(k,c1) we get:
31 XOR 30 = 01 (a padding value of 01 is valid. A one byte pad). At this point you know the last cleartext byte of m1. Keep this character since you will need it for future steps in this process.

### step 3: guess the next to last byte of m1 (this may take up to 256 attempts)

Going after the 2nd to last byte means that your pad will be two bytes : 0202.

Start guessing at this 2nd to last byte of m1 (g = 0x00, g = 0x01, etc) and start doing the algorithm noted in the picture below but this time you xor with 0x02 instead of 0x01. I'll show this calculation for the one correct answer below (where the padding oracle is returning a bad address but good pad).

In this case I would do the following: I already know the last byte of m1 and lets assume I now know the 2nd to last byte of m1 by querying the oracle.

To the last two bytes changes to c0 will be (g for last byte is 65 and I've determined g for 2nd byte is 20): 

(to be replaced in the c0 block. Note I'm XORing with 02 instead of 01 since I now need a 2 byte pad)
last byte:  
54 XOR 65 XOR 02 = 33

2nd to last byte: B1 XOR 20 XOR 02 = 93

So now when I XOR these with the corresponding intermediate results (D(k,c1)) I get:

33 XOR 31 = 02  
93 XOR 91 = 02  

### step 4: guess the next byte (this time you have a 3 byte pad: 030303)

### step N: keep going to find all of the plaintext m0 and m1

### step N+1: now you need to deal with the last block (the one with the real pad)

![alt text](https://github.com/pmPartch/CryptoI/raw/master/CBC_decode.PNG "AES with CBC")

## Use of C# to work the class optional assignments

I'm currently working the week 5 assignment and I continue to do all the assignments using C# (.Net 4.0 using Visual Studio 2010...I'm not using the more current Visual Studios only because they are so freaking slow to launch). Here are some notes about how to handle the various coding tasks.

### modulo arithmetic with C&#35;

Note that the modulo operator, %, will produce negative values. For instance, in the R language (and examples in class within the 'Notation' video at 2 minute mark) the following results can be obtained:  
(5-7) %% 12 = 10  
But in C# you will find  
(5-7) % 12 = -2  

You can work around this behavior by rewriting your modulo operations like so: (A % p + p) % p for some arithmetic results A. As a concrete example:  
((5-7) % 12 + 12) % 12 = 10

### implementation using AES by hand-coding your own CBC and CRT modes

You probably wish to setup a using for the System.Security.Cryptography namespace.

AesManaged does have a CBC implementation, but you are asked not to use it and instead implement your own CBC processing. Also, AesManaged does NOT have a counter mode (CRT) so you will be faced with implementation this yourself. Here are some notes:

Aes class is abstract, but you do have a factory in the static Create method of AesManaged.
But note that you will need to set the KeySize and BlockSize before assigning keys or IV values. So what I did was the following to create an Aes object in order to create a usable block cipher:

![alt text](https://github.com/pmPartch/CryptoI/raw/master/aes_factory.PNG "Aes Factory")

This Aes object can now be used to implement both the CBC and CRT solutions for assignment 2

### using .Net BigInteger class

Disclaimer: as I write this I have not yet solved the assignment 5...but I'm optimistic.

You need to reference the System.Numerics assembly and probably setup a using for the System.Numerics namespace.

C# BigInterger does have a modPow method, but does not have a modInverse and the mod operator is, well, a bit hidden.

You can work around the modInverse by using modPow like so:  
To calculate  __b ^ -1 mod m__ can be accomplished by __BigInteger.modPow(b,m-1,m)__

The modulus for BigInteger is an operator overload of the % operator...just be sure to have a BigInteger of either side of the operator.

$
