#ifndef MANGOS_H_MOVE_MAP_SHARED_DEFINES
#define MANGOS_H_MOVE_MAP_SHARED_DEFINES

#include "DetourNavMesh.h"

#define MMAP_MAGIC 0x4d4d4150   // 'MMAP'
#define MMAP_VERSION 4
#define SIZE_OF_GRIDS 533.33333f

struct MmapTileHeader
{
	unsigned int mmapMagic;
	unsigned int dtVersion;
	unsigned int mmapVersion;
	unsigned int size;
	bool usesLiquids : 1;

	MmapTileHeader() : mmapMagic(MMAP_MAGIC), dtVersion(DT_NAVMESH_VERSION),
		mmapVersion(MMAP_VERSION), size(0), usesLiquids(false) {}//usesLiquids(true) {} //Remove liquid in paths (not 100% with current maps)
};

enum NavTerrain
{
	NAV_EMPTY = 0x00,
	NAV_GROUND = 0x01,
	NAV_MAGMA = 0x02,
	NAV_SLIME = 0x04,
	NAV_WATER = 0x08,
	NAV_UNUSED1 = 0x10,
	NAV_UNUSED2 = 0x20,
	NAV_UNUSED3 = 0x40,
	NAV_UNUSED4 = 0x80
	// we only have 8 bits
};

#endif  // _MOVE_MAP_SHARED_DEFINES_H
