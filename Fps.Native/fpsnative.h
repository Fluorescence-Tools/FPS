// fpsnative.h
#include "exported.h"
#include <immintrin.h>
#include <xmmintrin.h>
#include <emmintrin.h>
#include <pmmintrin.h>
#include <x86intrin.h>
#include "math.h"

struct Vector3
{
	double x;
	double y;
	double z;
};

struct Matrix3
{
	double xx;	double xy;	double xz;
	double yx;	double yy;	double yz;
	double zx;	double zy;	double zz;
};

struct AtomCluster
{
	int NAtoms;
	int StartIndexInClusterArrays;
	Vector3 ClusterCenter;
	double ClusterRadius;
};

//////// AV ///////////

// main.cpp
extern "C"
{

EXPORTED int calculate1R(double L, double W, double R, int atom_i, double dg,	// linker and grid parameters
                double* XLocal, double* YLocal, double* ZLocal,						// atom coordinates
                double* vdWR, int NAtoms, double vdWRMax,							// v.d.Waals radii
                double linkersphere, int linknodes,									// linker routing parameters
                unsigned char* density);											// returns density array

EXPORTED int calculate3R(double L, double W, double R1, double R2, double R3, int atom_i, double dg,
                double* XLocal, double* YLocal, double* ZLocal,
                double* vdWR, int NAtoms, double vdWRMax,
                double linkersphere, int linknodes,
                unsigned char* density);

EXPORTED double rdamean(Vector3* av1, int av1length, Vector3* av2, int av2length, int nsamples, int rndseed);

EXPORTED double rdameanE(Vector3* av1, int av1length, Vector3* av2, int av2length, int nsamples, int rndseed, double R0);

///////// Clashes /////////

EXPORTED double checkforclashes(__m128* m1xyzv, __m128* m2xyzv,	AtomCluster* ac1, AtomCluster* ac2,
                       int nclusters1, int nclusters2, int* mustbechecked1, int* mustbechecked2, Matrix3 rotation2to1, Vector3 dcm,
                       double kchash, Vector3& clashforce, Vector3& clashtorque1, Vector3& clashtorque2);

//////// misc /////////////

EXPORTED int testnative();

}


