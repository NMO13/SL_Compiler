#include "stdafx.h"
#include "CppUnitTest.h"
#include <stdio.h>
#include <memory.h>
#include <windows.h>
#include <assert.h>
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SL_Compiler_Tests
{
	char data[1<<16];

		// __stdcall makes the callee pop the arguments from the stack upon return
		void __stdcall Put(char ch) 
		{
			printf("%c", ch);
		}
		void __stdcall PutLn() 
		{
			printf("\n");
		}

		void __stdcall Assert(int exp, int act)
		{
			Assert::AreEqual(exp, act);
		}

		#pragma endregion 

		void ExecuteFile(char* filename)
		{
			int i = 0x0FFFFFFF;
			int j = 0x0FFFFFFF;
			int z = i * j;
				/* load the code file */ 
			FILE* fin;
			fopen_s(&fin, filename, "rb");
			if (fin == NULL) 
			{
				printf("couldn't open %s\n", filename);
				Assert::Fail();
			}

			byte* code = (byte*) VirtualAllocEx(GetCurrentProcess(), 0, 1<<16, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
			size_t size = fread(code, 1, 1<<16, fin);
			fclose(fin);

			printf("loaded %i bytes\n", size);
			printf("executing...\n");
	
			/* setup the global data block */
			memset(data, 0, sizeof(data));
			((void**)data)[0] = (void*)Put;
			((void**)data)[1] = (void*)PutLn;
			((void**)data)[2] = (void*)Assert;

			/* set edi and jump to the code block */
			void* start = code;
			#ifdef __GNUC__
			  __asm__ ("call *%%eax" : /* */ : "a" (start), "D" (data) : /* */ );
			#else
				__asm {
					lea edi, data;
 					call start;
				};
				
			
			#endif
		}


	TEST_CLASS(UnitTest1)
	{
	public:
		
		TEST_METHOD(GlobalAssignment_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\GlobalAssignment.obj");
		}
		
		TEST_METHOD(GlobalAssignment2_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\GlobalAssignment2.obj");
		}

		TEST_METHOD(LocalAssignment_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\LocalAssignment.obj");
		}

		TEST_METHOD(SimpleAdd_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\SimpleAdd.obj");
		}

		TEST_METHOD(SimpleMeth_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\SimpleMeth.obj");
		}

		TEST_METHOD(SimpleMeth2_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\SimpleMeth2.obj");
		}

		TEST_METHOD(PutTest_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\PutTest.obj");
		}

		TEST_METHOD(Param1_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\Param1.obj");
		}

		TEST_METHOD(ManyParams_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\ManyParams.obj");
		}

		TEST_METHOD(Locals_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\Locals.obj");
		}

		TEST_METHOD(SimpleExpression1_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\SimpleExpression1.obj");
		}

		TEST_METHOD(SimpleExpression2_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\SimpleExpression2.obj");
		}

		TEST_METHOD(SimpleExpression3_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\SimpleExpression3.obj");
		}

		TEST_METHOD(SimpleExpression4_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\SimpleExpression4.obj");
		}

		TEST_METHOD(Branch1_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\Branch1.obj");
		}

		TEST_METHOD(Branch2_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\Branch2.obj");
		}

		TEST_METHOD(While1_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\While1.obj");
		}

		TEST_METHOD(ORDCHRTest_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\ORDCHRTest.obj");
		}

		TEST_METHOD(BiggerExample_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\BiggerExample.obj");
		}

		TEST_METHOD(Faculty_Code)
		{
			ExecuteFile("C:\\Martin\\Uni\\Übersetzerbau2\\SL-Compiler\\SL_Compiler\\Tests\\CodeGeneration\\Faculty.obj");
		}
	};
}