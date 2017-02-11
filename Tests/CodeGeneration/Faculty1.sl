PROGRAM
VAR i: INTEGER;
	PROCEDURE Fac(x: INTEGER) : INTEGER;
	BEGIN
		IF x = 0 THEN
			RETURN 1
		ELSE
			RETURN x * Fac(x-1) // fac returns in eax but eax is already used by multiplication
		END;
	END Fac;
BEGIN
	i := Fac(10);
	Assert(i, 3628800);
END.