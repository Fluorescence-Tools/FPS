#include "fpsnative.h"

#pragma warning(disable: 4244) // reviewed

inline __m128 cross(__m128 a, __m128 b)
{
	// (a.Y*b.Z - a.Z*b.Y, a.Z*b.X - a.X*b.Z, a.X*b.Y - a.Y*b.X, undefined)
	__m128 ashuf = _mm_shuffle_ps(a, a, _MM_SHUFFLE(0, 0, 2, 1));
	__m128 bshuf = _mm_shuffle_ps(b, b, _MM_SHUFFLE(0, 0, 2, 1));
	__m128 cshuf = _mm_sub_ps(_mm_mul_ps(a, bshuf), _mm_mul_ps(b, ashuf));
	return _mm_shuffle_ps(cshuf, cshuf, _MM_SHUFFLE(0, 0, 2, 1));
}

double checkforclashes( __m128* m1xyzv,		// molecule 1 => __m128 (x y z vdw)
						__m128* m2xyzv,
						AtomCluster* ac1,
						AtomCluster* ac2,
						int nclusters1, int nclusters2,
						const int* mustbechecked1, const int* mustbechecked2,
						Matrix3 rotation2to1, Vector3 dcm, double kchash,
						Vector3& clashforce, Vector3& clashtorque1, Vector3& clashtorque2)
{
	int ntocheck2 = 0, natoms2 = 0;
    __attribute__((aligned(16))) float m128dump[4];

	// rotation matrix and translation vectors
	__m128 rotmv[3] = { _mm_setr_ps(rotation2to1.xx, rotation2to1.yy, rotation2to1.zz, -1.), // this will load -vdrWR
		_mm_setr_ps(rotation2to1.xy, rotation2to1.yz, rotation2to1.zx, 0.),
		_mm_setr_ps(rotation2to1.xz, rotation2to1.yx, rotation2to1.zy, 0.) };
	__m128 dcmv = _mm_setr_ps(dcm.x, dcm.y, dcm.z, 0.);

	// load clusters of m2, at the same time rotate and shift
	__m128* ac2_vec = new __m128[nclusters2];
	for (int j = 0; j < nclusters2; j++)
	{
		if (!mustbechecked2[j]) continue;
		__m128 c = _mm_setr_ps(ac2[j].ClusterCenter.x, ac2[j].ClusterCenter.y,
			ac2[j].ClusterCenter.z, ac2[j].ClusterRadius);
		__m128 vshuf1 = _mm_shuffle_ps(c, c, _MM_SHUFFLE(1, 0, 2, 1));
		__m128 vshuf2 = _mm_shuffle_ps(c, c, _MM_SHUFFLE(1, 1, 0, 2));
		__m128 t1 = _mm_add_ps(_mm_mul_ps(rotmv[0], c), dcmv);
		__m128 t2 = _mm_add_ps(_mm_mul_ps(rotmv[1], vshuf1), _mm_mul_ps(rotmv[2], vshuf2));
		ac2_vec[ntocheck2++] = _mm_add_ps(t1, t2);
		natoms2 += ac2[j].NAtoms;
	}

	// rotate and shift all atoms of "MustBeChecked" clusters of m2 (2 -> 1)
	__m128* m2xyzr_vec = new __m128[natoms2];
	int* ac2start = new int[ntocheck2 + 1]; ac2start[0] = 0;
	int iac2 = 0, iatomm2 = 0;
	for (int j = 0; j < nclusters2; j++)
	{
		if (!mustbechecked2[j]) continue;
		for (int aj = 0; aj < ac2[j].NAtoms; aj++)
		{
			__m128 a = m2xyzv[ac2[j].StartIndexInClusterArrays + aj];
			__m128 vshuf1 = _mm_shuffle_ps(a, a, _MM_SHUFFLE(1, 0, 2, 1));
			__m128 vshuf2 = _mm_shuffle_ps(a, a, _MM_SHUFFLE(1, 1, 0, 2));
			__m128 t1 = _mm_add_ps(_mm_mul_ps(rotmv[0], a), dcmv);
			__m128 t2 = _mm_add_ps(_mm_mul_ps(rotmv[1], vshuf1), _mm_mul_ps(rotmv[2], vshuf2));
			m2xyzr_vec[iatomm2++] = _mm_add_ps(t1, t2);
		}
		ac2start[++iac2] = iatomm2;
	}

    // actual checking for clashes
	__m128 kclash_vec = _mm_setr_ps(kchash, kchash, kchash, 0.);
	__m128 force_vec = _mm_setzero_ps(), torque1_vec = _mm_setzero_ps(), torque2_vec = _mm_setzero_ps(),
		eclash_vec = _mm_setzero_ps(), absforce;
	__m128 clearv012 = _mm_cmpeq_ps(kclash_vec, force_vec);
	__m128 clearv3 = _mm_cmpneq_ps(kclash_vec, force_vec);
	for (int i = 0; i < nclusters1; i++)
	{
		if (!mustbechecked1[i]) continue;
		for (int ai = 0; ai < ac1[i].NAtoms; ai++)
		{
			__m128 a1 = m1xyzv[ac1[i].StartIndexInClusterArrays + ai];
			for (int j = 0; j < ntocheck2; j++)
			{
				// against whole clusters of m2
				__m128 d = _mm_sub_ps(a1, ac2_vec[j]);  // (dx, dy, dz, vdw1+vdw2)
				__m128 dsq = _mm_mul_ps(d, d);
				__m128 t = _mm_hadd_ps(_mm_and_ps(dsq, clearv3), _mm_and_ps(dsq, clearv012));  // (dx^2+dy^2, dz^2, 0, sumvdw^2)
				__m128 t2 = _mm_hadd_ps(t, t); // (dx^2+dy^2+dz^2, sumvdw^2, dx^2+dy^2+dz^2, sumvdw^2)
				if (_mm_ucomilt_ss(t2, _mm_movehdup_ps(t2))) // |r|^2 < sumvdw^2
				{
					for (int aj = ac2start[j]; aj < ac2start[j+1]; aj++)
					{
						// against all atoms
						__m128 a2 = m2xyzr_vec[aj];
						d = _mm_sub_ps(a1, a2); 
						dsq = _mm_mul_ps(d, d);
						t = _mm_hadd_ps(_mm_and_ps(dsq, clearv3), _mm_and_ps(dsq, clearv012));
						t2 = _mm_hadd_ps(t, t);
						if (_mm_ucomilt_ss(t2, _mm_movehdup_ps(t2))) // clash
						{
							t = _mm_sqrt_ps(t2); // (|r|, sumvdw, |r|, sumvdw)
							t2 = _mm_moveldup_ps(t); // (|r| x 4)
							__m128 clashviol = _mm_sub_ps(_mm_movehdup_ps(t), t2); // sumvdw - |r|
							absforce = _mm_mul_ps(clashviol, kclash_vec); // (|force| x 3, 0)
							eclash_vec = _mm_add_ps(eclash_vec, _mm_mul_ps(absforce, clashviol));
							__m128 f = _mm_mul_ps(d, _mm_mul_ps(absforce, _mm_rcp_ps(t2))); // force vector for this atom pair
							force_vec = _mm_add_ps(force_vec, f);
							torque1_vec = _mm_add_ps(torque1_vec, cross(a1, f));
							torque2_vec = _mm_sub_ps(torque2_vec, cross(_mm_sub_ps(a2, dcmv), f));
						}
					}
				}
			}
		}
	}

	delete[] ac2_vec; delete[] m2xyzr_vec; delete[] ac2start;
	
	// copy results to output
	_mm_store_ps(m128dump, force_vec);
	clashforce.x = m128dump[0]; clashforce.y = m128dump[1]; clashforce.z = m128dump[2];
	_mm_store_ps(m128dump, torque1_vec);
	clashtorque1.x = m128dump[0]; clashtorque1.y = m128dump[1]; clashtorque1.z = m128dump[2];
	_mm_store_ps(m128dump, torque2_vec);
	clashtorque2.x = m128dump[0]; clashtorque2.y = m128dump[1]; clashtorque2.z = m128dump[2];
	_mm_store_ps(m128dump, eclash_vec);
	return 0.5 * m128dump[0];
}
