<h1 align="center">CEE</h1>

<p align="center"><strong>This readme is modified from KEE and is temporary.</strong></p>

<p align="center"><strong>Future instructions will likely be in <a href="https://kee.sharkb.yt/#/wiki/Implementations/CEE_(C_)">the CEE section of the KEE wiki.</a></strong></p>

# Topics

1. [About](#about)
2. [Installation](#installation)
    - [From the Releases](#from-the-releases)
3. [Usage](#usage)
    - [Encrypting/Decrypting files with a key](#encryptingdecrypting-files-with-a-key)
    - [Writing your own keys](#writing-your-own-keys)
        - [About CEED](#about-ceed)
        - [CEED Syntax](#ceed-syntax)
        - [Base Concepts](#base-concepts)
        - [CEED Instructions](#ceed-instructions)
        - [CEED Arguments](#ceed-arguments)
        - [Compiling and decompiling](#compiling-and-decompiling)

# About

CEE is a [KEE](https://kee.sharkb.yt/#/wiki/Main_Page) implementation with a few extra features. This includes modified source and key formats, as well as additional instructions.

All files encrypted with KEE can be decrypted with the correct CEE key, but the opposite is not true.

All credit for KEE and the KEE standard goes to [Jae](https://sharkb.yt)

> [!WARNING]
> At this time, CEE is more of a novelty than an actual file encryption program, and has not been thoroughly tested. Always ensure your files are safely backed up, or don't overwrite the input file.
>
> I am not responsible for any lost data.

# Installation

## From the Releases

1. Download the [latest releases](https://github.com/TheZoidMaster/CEE/releases/latest) of CEE for your operating system.

2. (Optional) Add the executable to PATH or environment variables so you can use it everywhere.

3. **Done!**

> [!NOTE]
> CEE requires .NET >= 9

# Usage

This section will explain every feature of CEE and CEED. If you want to skip to a specific part, look [here](#topics)!

## Encrypting/Decrypting Files with a key

Let's say we have an example file, `my_file.txt`, and it contains the following content:

```txt
My day has been going great!
```

We can now encrypt our text file with a key called `my_key.cee`!

```bash
cee file my_file.txt my_key.cee [output_file.txt] [-d]
```

> [!NOTE]
> Specifying an output is optional; however, if you choose not to, the input file will be overwritten.
> 
> Appending -d anywhere in the command will switch CEE into decryption mode, getting back the original file.

---

## Writing your own keys

### About CEED

CEED is the encryption language in which keys are written. It's a human-readable version of the `.cee` format, with some additional features. It compiles to `.cee`, which then can be used for encryption. CEED is similar to assembly, but there are differences that we'll talk about in a bit.

### CEED Syntax

The syntax for CEED is quite basic, easy to learn, and easy to understand, even for beginner programmers.

Every line is formatted like a variation of the following syntax:
```bash
instruction <arguments>
```
Sometimes, there aren't any arguments required at all
```bash
instruction
```
Of course, you're also able to write comments.
```bash
# Here's a comment!
instruction # They also support being inline
```

### Base Concepts

Whenever a byte exceeds `0xff`, it loops back around to `0x00`. The same applies in reverse.

### CEED Instructions

-   `LGD`: Adds a linear gradient on top of the file, input 1 is the starting value, input 2 is the ending value.
-   `GRD`: Adds multiple linear gradients on top of the file, evenly spaced apart. This can have any number of inputs above 1.
-   `ADD`: Adds a number to every byte.
-   `SUB`: Subtracts a number from every byte.
-   `LSH`: Applies a left-shift to every byte.
-   `RSH`: Applies a right-shift to every byte.

### CEED Arguments

Arguments in CEED are quite simple.
```bash
add 0xff
```
```bash
add ff
```
Both of these would add 255 to every byte in the file.

Aside from standard hex arguments, there are also special keywords that can be used. All of these have special behavior when the key is compiled.

-   `RND`: Turns into a random number.
    - Input is optional, specifies maximum value.
-   `UTC`: Turns into the current UTC hour
    - No input.
-   `DEC`: Allows decimal input.
    - Input is a decimal number.
-   `BIN`: Allows binary input.
    - Input is a binary number.

> [!WARNING]
> Since these get calculated during key compilation, they cannot be recovered via a decompile.
>
> Decompiled keys that had special keywords will only contain the hex codes that they became.

Here's an example without input:
```bash
add rnd # This could be anything 0x00-0xff
```
And here's one with input:
```bash
add DEC(64) # Gets converted into 0x40
```

### Compiling and decompiling

Once you're done writing your key, you can compile it with the following command:

```bash
cee compile my_key.ceed [cool_key.cee]
```
> [!NOTE]
> Specifying an output is optional. If you choose not to, the output file will be named the same as the input, but with `.cee` as the file extension.

If you have a key with no source, you can also decompile it with this command:
```bash
cee decompile my_key.cee [cool_key.ceed]
```
> [!NOTE]
> Specifying an output is optional. If you choose not to, the output file will be named the same as the input, but with `.ceed` as the file extension.
