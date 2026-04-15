using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DeveMazeGeneratorCore.Mazes;

// Based on https://blog.stephencleary.com/2023/09/memory-mapped-files-overlaid-structs.html
public unsafe ref struct Overlay : IDisposable
{
    private readonly MemoryMappedViewAccessor _view;
    
    public Span<byte> Array;

    public Overlay(MemoryMappedViewAccessor view)
    {
        _view = view;
        byte* _pointer;
        view.SafeMemoryMappedViewHandle.AcquirePointer(ref _pointer);
        Array = new Span<byte>(_pointer, (int)view.SafeMemoryMappedViewHandle.ByteLength);
    }

    public void Dispose()
    {
        _view.SafeMemoryMappedViewHandle.ReleasePointer();
        _view.Dispose();
    }
}
