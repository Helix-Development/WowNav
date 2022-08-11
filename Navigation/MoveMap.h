#ifndef MANGOS_H_MOVE_MAP
#define MANGOS_H_MOVE_MAP

#include <unordered_map>
#include <map>

#include "DetourAlloc.h"
#include "DetourNavMesh.h"
#include "DetourNavMeshQuery.h"

#include "Utilities/UnorderedMapSet.h"

//  memory management
inline void* dtCustomAlloc(int size, dtAllocHint /*hint*/)
{
	return (void*)new unsigned char[size];
}

inline void dtCustomFree(void* ptr)
{
	delete[](unsigned char*)ptr;
}

namespace MMAP
{
	typedef std::unordered_map<unsigned int, dtTileRef> MMapTileSet;
	typedef std::unordered_map<unsigned int, dtNavMeshQuery*> NavMeshQuerySet;

	struct MMapData
	{
		MMapData(dtNavMesh* mesh) : navMesh(mesh) {}
		~MMapData()
		{
			for (NavMeshQuerySet::iterator i = navMeshQueries.begin(); i != navMeshQueries.end(); ++i)
			{
				dtFreeNavMeshQuery(i->second);
			}

			if (navMesh)
			{
				dtFreeNavMesh(navMesh);
			}
		}

		dtNavMesh* navMesh;

		// we have to use single dtNavMeshQuery for every instance, since those are not thread safe
		NavMeshQuerySet navMeshQueries;     // instanceId to query
		MMapTileSet mmapLoadedTiles;        // maps [map grid coords] to [dtTile]
	};

	typedef std::unordered_map<unsigned int, MMapData*> MMapDataSet;

	class MMapManager
	{
	public:
		~MMapManager();

		std::map<unsigned int, bool> zoneMap = {};

		bool loadMap(unsigned int mapId, int x, int y);

		// the returned [dtNavMeshQuery const*] is NOT threadsafe
		dtNavMeshQuery const* GetNavMeshQuery(unsigned int mapId, unsigned int instanceId);
		dtNavMesh const* GetNavMesh(unsigned int mapId);

		unsigned int getLoadedMapsCount() const { return loadedMMaps.size(); }
	private:
		bool loadMapData(unsigned int mapId);
		unsigned int packTileID(int x, int y);

		MMapDataSet loadedMMaps;
	};

	class MMapFactory
	{
	public:
		static MMapManager* createOrGetMMapManager();
	};
}

#endif
