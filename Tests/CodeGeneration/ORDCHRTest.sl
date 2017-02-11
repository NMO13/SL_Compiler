PROGRAM
VAR i : INTEGER;
VAR c : CHAR;
BEGIN
ORD("c");
CHR(i);
i := 49;
c := CHR(i);
i := ORD(c);
Assert(i, 49);
END.