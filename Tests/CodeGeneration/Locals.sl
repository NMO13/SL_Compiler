PROGRAM
	VAR i : INTEGER;
    PROCEDURE Foo() : INTEGER;
		VAR i0, i1 : INTEGER; VAR c0, c1 : CHAR;
	BEGIN
		i0 := 10;
		RETURN i0;
	END Foo;
BEGIN
	i := Foo();
	Assert(i, 10);
END.