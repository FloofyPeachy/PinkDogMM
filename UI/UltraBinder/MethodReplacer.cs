using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PinkDogMM_Gd.UltraBinder;

public class MethodReplacer
{
    public static void install(MethodInfo methodToReplace, MethodInfo methodToInject)
    {
        //  MethodInfo methodToReplace = typeof(Target).GetMethod("targetMethod"+ funcNum, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        //  MethodInfo methodToInject = typeof(Injection).GetMethod("injectionMethod"+ funcNum, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
        RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);

        unsafe
        {
            if (IntPtr.Size == 4)
            {
                int* inj = (int*)methodToInject.MethodHandle.Value.ToPointer() + 2;
                int* tar = (int*)methodToReplace.MethodHandle.Value.ToPointer() + 2;
#if DEBUG
                Console.WriteLine("\nVersion x86 Debug\n");

                byte* injInst = (byte*)*inj;
                byte* tarInst = (byte*)*tar;

                int* injSrc = (int*)(injInst + 1);
                int* tarSrc = (int*)(tarInst + 1);

                *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                    Console.WriteLine("\nVersion x86 Release\n");
                    *tar = *inj;
#endif
            }
            else
            {

                long* inj = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
#if DEBUG
                Console.WriteLine("\nVersion x64 Debug\n");
                byte* injInst = (byte*)*inj;
                byte* tarInst = (byte*)*tar;


                int* injSrc = (int*)(injInst + 1);
                int* tarSrc = (int*)(tarInst + 1);

                *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                    Console.WriteLine("\nVersion x64 Release\n");
                    *tar = *inj;
#endif
            }
        }
    }
}