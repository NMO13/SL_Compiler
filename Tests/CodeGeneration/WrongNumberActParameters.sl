PROGRAM
	VAR i : INTEGER;
    PROCEDURE Foo(VAR i, j : INTEGER) : INTEGER;
	BEGIN
		RETURN i;
	END Foo;
BEGIN
	i := Foo(3);
	i := Foo(3, 4, 5);
END.