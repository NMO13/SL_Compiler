PROGRAM
VAR i, j:INTEGER;
BEGIN
i := 268435455;
j := 268435455;
i := i * j;
Assert(i, -536870911);

i := 10;
j := i * 400;
Assert(j, 4000);

j := 10;
j := -j * j;
Assert(j, -100);

i := 10;
j := 5;
i := j / i;
Assert(i, 0);

j := j / 2;
Assert(j, 2);

j := 16;
j := - j / 2 / 2;
Assert(j, -4);

i := 3;
j := 4;

i := -i * i * j;
Assert(i, -36);
END.