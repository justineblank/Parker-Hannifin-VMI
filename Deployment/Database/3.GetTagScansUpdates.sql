CREATE PROCEDURE GetTagScans
(
@location		VARCHAR(256),
@earliestScan	DATETIME = NULL
)
AS
BEGIN
	DECLARE @d_TimeStamp_UTC AS DATETIME = GETUTCDATE()
	--
	
		SELECT 
			epc, [timestamp]
		FROM
		Readers WITH(NOLOCK)
		INNER JOIN 
		Scans WITH(NOLOCK)
		ON Readers.id = Scans.readerId

		WHERE 
		(
			Readers.[location] = @location
			or
			Readers.[readerId] = @location
		)
		AND
		Scans.[timestamp] >= IsNull(@earliestScan,Scans.[timestamp])

		/*************************************************************/

		UPDATE Scans
		SET Scans.[syncStatus] = @d_TimeStamp_UTC
		FROM
		Readers 
		INNER JOIN 
		Scans
		ON Readers.id = Scans.readerId

		WHERE 
		(
			Readers.[location] = @location
			or
			Readers.[readerId] = @location
		)
		AND
		Scans.[timestamp] >= IsNull(@earliestScan,Scans.[timestamp])
	
END
