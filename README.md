# HoLLy.MemoryLib
A C# memory-accessing library

## Warning
This is not meant to be a general-purpose library. It is not meant to have a stable API or be feature-complete. This
is simply what I used or needed in some of my projects. I'm not interested in PRs or issues related to feature requests,
and no support will be given.

## Features
- Signature scanning, accepting Cheat Engine style patterns.
- OOP-like memory access

### Signature Scanning
You give it a pattern like `"DE AD BE EF ?? ?? ?? ?? CA FE BA BE"` and it will return you the first occurence of this
pattern in the process' memory. Optionally, you can also pass required memory page properties such as protection or state.

### OOP Memory Access
You can "map" memory structures in an external process and access them in an intuitive manner.

```cs
internal class MyExternalObject : StructureBase
{
    public MyExternalObject(MemoryAbstraction mem, IntPtr addr) : base(mem, addr) { }

    protected override int PrefetchSize => 0x95; // The size of the object, if you wish to cache it

    public float Value1 => Float(0x10);        // 32bit floating point number
    public int Value2 => Int(0x20);            // 32bit integer
    public string Value3 => String(0x30);      // .NET String
    public MyEnum Value4 => (MyEnum)Int(0x40);

    public MyOtherObject OtherObject => new MyOtherObject(Memory, Pointer(0xD0));    // MyOtherObject is another StructureBase

    public List<MyOtherObject> List1 => this.List<MyOtherObject>(0x48);               // .NET lists are supported
    public List<MySecondOtherObject> List1 => this.List<MySecondOtherObject>(0x48);   // Union-type fields naturally work too
}
```

## License
This project is licensed under the permissive MIT license.
