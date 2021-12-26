﻿CREATE TABLE [dbo].[TemporalCar]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[SysStart] DATETIME2 (7) GENERATED ALWAYS AS ROW START NOT NULL,
	[SysEnd] DATETIME2 (7) GENERATED ALWAYS AS ROW END NOT NULL,
	PERIOD FOR SYSTEM_TIME ([SysStart], [SysEnd])
)
WITH (SYSTEM_VERSIONING = ON(HISTORY_TABLE=[dbo].[TemporalCar_HISTORY], DATA_CONSISTENCY_CHECK=ON))
