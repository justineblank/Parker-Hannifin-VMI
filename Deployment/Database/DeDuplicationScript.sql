/*remove all scans from the database where that epc was ‘seen’ less than 36 hours ago*/

	delete from Scans
	where id in (	
			select distinct S1.id--,S1.epc ,s1.[timestamp], S2.[timestamp] --,dateadd(HOUR, 36, S2.[timestamp]) as [+36],S2.id
				from Scans S1 with(nolock)
				inner join 
				Scans S2 with(nolock)
				on S1.epc = S2.epc
				and 
					S1.[timestamp] <  dateadd(HOUR, 36, S2.[timestamp])
					and 
					S1.[timestamp] >  S2.[timestamp]

	)