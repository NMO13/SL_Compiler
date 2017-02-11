PROGRAM
    VAR i : INTEGER;
    PROCEDURE Foo(VAR i : INTEGER) : INTEGER;
	BEGIN
		RETURN i;
	END Foo;
BEGIN
	i := Foo(3);
	Assert(i, 3)
END.
