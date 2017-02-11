// Loader.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <stdio.h>
#include <memory.h>
#include <windows.h>

char data[1<<16];

// __stdcall makes the callee pop the arguments from the stack upon return
void __stdcall Put(char ch) {
	printf("%c", ch);
}
void __stdcall PutLn() {
	printf("\n");
}

void CheckPath(char** p)
{
	int i = 0;
	int c = p[1][i];
	while(c != 0)
	{
		printf("%c", c);
		c = p[1][++i];
	}
}
int main(int argc, char* argv[])
{
	/* check arguments */
	if (argc != 2) {
		printf("usage: %s <binary file>\n", argv[0]);
		return -1;
	}

	/* load the code file */ 
	FILE* fin;
	fopen_s(&fin, argv[1], "rb");
	if (fin == NULL) {
		printf("couldn't open %s\n", argv[1]);
		return -2;
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

	return 0;
}