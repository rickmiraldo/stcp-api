# 🚌🚏 Unofficial STCP API - Porto (PT) buses

This software aims to create an unofficial API for STCP buses in the Porto metropolitan area.

Since there's no official API, this software relies on simple HTTP requests to an undocumented STCP endpoint, and as such, is subject to break if any major changes happens.

Also, it is subject to the same limitations as the STCP endpoint itself, for example, not displaying more than 4 buses at a time and not displaying any bus if their ETA is more than 60 minutes.

Built with .Net Core 3.

## Usage

To get all buses from a stop use this API call:

`<server>:<port>/stop/<code>`

Where `<code>` is the bus stop code such as CMIC1 or LION1

## Credits

 * Using `html-agility-pack` from `zzzprojects` | [GitHub link](https://github.com/zzzprojects/html-agility-pack)
 * Code by me
