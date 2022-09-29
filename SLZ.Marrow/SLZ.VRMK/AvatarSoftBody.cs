using UnityEngine;

namespace SLZ.VRMK
{
    public partial class Avatar : MonoBehaviour
    {
        [System.Serializable]
        public struct SoftEllipse
        {
            public struct Constants
            {
                public const float RadiusMin = 0.01f;
                public const float RadiusMax = 0.1f;
                public const float RadiusDefault = 0.03f;
                public const float BiasMin = -1f;
                public const float BiasMax = 1f;
                public const float BiasDefault = 0f;
            }

            [Range(Constants.RadiusMin, Constants.RadiusMax)]
            public float XRadius;
            [Range(Constants.BiasMin, Constants.BiasMax)]
            public float XBias;
            [Range(Constants.RadiusMin, Constants.RadiusMax)]
            public float ZRadius;
            [Range(Constants.BiasMin, Constants.BiasMax)]
            public float ZBias;

            public SoftEllipse(float xRadius = Constants.RadiusDefault, float xBias = Constants.BiasDefault, float zRadius = Constants.RadiusDefault, float zBias = Constants.BiasDefault)
            {
                XRadius = xRadius;
                XBias = xBias;
                ZRadius = zRadius;
                ZBias = zBias;
            }


        }
        [System.Serializable]
        public class SoftBulge
        {
            [Range(0f, 90f)]
            public float apexX;
            [Range(.01f, .99f)]
            public float apexY;
            [Range(0f, .08f)]
            public float apexZ;
            [Range(0f, 1f)]
            public float upperY;
            [Range(0f, 1f)]
            public float lowerY;
            [Range(0f, 80f)]
            public float innerX;
            [Range(0f, 80f)]
            public float outerX;
            [Range(0f, 1f)]
            public float roundUpperInner;
            [Range(0f, 1f)]
            public float roundUpperOuter;
            [Range(0f, 1f)]
            public float roundLowerInner;
            [Range(0f, 1f)]
            public float roundLowerOuter;
            [Range(0f, .5f)]
            public float swellUpper;
            [Range(0f, .5f)]
            public float swellLower;
            [Range(0f, .5f)]
            public float swellInner;
            [Range(0f, .5f)]
            public float swellOuter;
            [Range(0f, 1f)]
            public float rigidity;

            public Transform primaryRt;
            public Transform secondaryLf;









            public Vector3 ComputeBulge(float yPerc, Vector2 sin, float bulgeYTop, float bulgeYBot, bool onBack)
            {
                float apexYAbs = Mathf.LerpUnclamped(bulgeYTop, bulgeYBot, apexY);
                float topToBotDiff = bulgeYBot - bulgeYTop;
                float minY = apexYAbs - (topToBotDiff * apexY) * upperY;
                float maxY = apexYAbs + (topToBotDiff * (1f - apexY)) * lowerY;
                if (onBack) sin.y = -sin.y;
                if (yPerc > maxY || yPerc < minY) return Vector3.zero;


                float yMultInv = 0f;
                float xMultInv = 0f;

                float xOffset = 0f;
                float yOffset = 0f;
                float zOffset = 0f;

                float innerRadians = (apexX - innerX) * Mathf.Deg2Rad;
                float apexRadians = apexX * Mathf.Deg2Rad;
                float outerRadians = (apexX + outerX) * Mathf.Deg2Rad;

                float innerSin = Mathf.Sin(innerRadians);
                float apexCos = Mathf.Cos(apexRadians);
                float apexSin = Mathf.Sin(apexRadians);
                float outerCos = Mathf.Cos(outerRadians);

                if (yPerc < apexYAbs)
                {
                    float yMult = Mathf.InverseLerp(apexYAbs, minY, yPerc);
                    yMultInv = 1f - yMult;

                    if (sin.y < apexCos)
                    {
                        float xMult = Mathf.InverseLerp(apexCos, Mathf.LerpUnclamped(outerCos, apexCos, yMult * yMult * roundUpperOuter), sin.y);
                        xMultInv = 1f - xMult;

                        float swellXMult = Mathf.Abs(xMult - .5f) * 2f;
                        swellXMult = 1f - (swellXMult * swellXMult);
                        float swellX = swellOuter * apexZ * swellXMult * yMultInv;
                        zOffset += swellX;
                        xOffset += swellX;
                    }
                    else
                    {
                        float xMult = Mathf.InverseLerp(apexSin, Mathf.LerpUnclamped(innerSin, apexSin, yMult * yMult * roundUpperInner), Mathf.Abs(sin.x));
                        xMultInv = 1f - xMult;

                        float swellXMult = Mathf.Abs(xMult - .5f) * 2f;
                        swellXMult = 1f - (swellXMult * swellXMult);
                        float swellX = swellInner * apexZ * swellXMult * yMultInv;
                        zOffset += swellX;
                        xOffset -= swellX;
                    }

                    float swellYMult = Mathf.Abs(yMult - .5f) * 2f;
                    swellYMult = 1f - (swellYMult * swellYMult);
                    float swellY = swellUpper * apexZ * swellYMult * xMultInv;
                    zOffset += swellY;
                    yOffset += swellY;
                }
                else
                {
                    float yMult = Mathf.InverseLerp(apexYAbs, maxY, yPerc);
                    yMultInv = 1f - yMult;

                    if (sin.y < apexCos)
                    {
                        float xMult = Mathf.InverseLerp(apexCos, Mathf.LerpUnclamped(outerCos, apexCos, yMult * yMult * roundLowerOuter), sin.y);
                        xMultInv = 1f - xMult;

                        float swellXMult = Mathf.Abs(xMult - .5f) * 2f;
                        swellXMult = 1f - (swellXMult * swellXMult);
                        float swellX = swellOuter * apexZ * swellXMult * yMultInv;
                        zOffset += swellX;
                        xOffset += swellX;
                    }
                    else
                    {
                        float xMult = Mathf.InverseLerp(apexSin, Mathf.LerpUnclamped(innerSin, apexSin, yMult * yMult * roundLowerInner), Mathf.Abs(sin.x));
                        xMultInv = 1f - xMult;

                        float swellXMult = Mathf.Abs(xMult - .5f) * 2f;
                        swellXMult = 1f - (swellXMult * swellXMult);
                        float swellX = swellInner * apexZ * swellXMult * yMultInv;
                        zOffset += swellX;
                        xOffset -= swellX;
                    }

                    float swellYMult = Mathf.Abs(yMult - .5f) * 2f;
                    swellYMult = 1f - (swellYMult * swellYMult);
                    float swellY = swellLower * apexZ * swellYMult * xMultInv;
                    zOffset += swellY;
                    yOffset -= swellY;
                }

                Vector3 softOffset = new Vector3((apexZ * yMultInv * xMultInv + zOffset) * sin.x, yOffset, (apexZ * yMultInv * xMultInv + zOffset) * sin.y);
                softOffset.x += xOffset * sin.y * Mathf.Sign(sin.x);
                softOffset.z += xOffset * sin.x * Mathf.Sign(sin.x);




                if (onBack) softOffset.z = -softOffset.z;
                return softOffset;
            }
            public SoftBulge()
            {
                this.apexX = 26f;
                this.apexY = .5f;
                this.apexZ = 0f;
                this.upperY = 1f;
                this.lowerY = 1f;
                this.innerX = 26f;
                this.outerX = 32f;
                this.roundUpperInner = .1f;
                this.roundUpperOuter = 0f;
                this.roundLowerInner = .9f;
                this.roundLowerOuter = .4f;
                this.swellUpper = .1f;
                this.swellLower = .2f;
                this.swellInner = .2f;
                this.swellOuter = .1f;
                this.rigidity = .5f;
            }
            public void Copy(SoftBulge bulge)
            {
                apexX = bulge.apexX;
                apexY = bulge.apexY;
                apexZ = bulge.apexZ;
                upperY = bulge.upperY;
                lowerY = bulge.lowerY;
                innerX = bulge.innerX;
                outerX = bulge.outerX;
                roundUpperInner = bulge.roundUpperInner;
                roundUpperOuter = bulge.roundUpperOuter;
                roundLowerInner = bulge.roundLowerInner;
                roundLowerOuter = bulge.roundLowerOuter;
                swellUpper = bulge.swellUpper;
                swellLower = bulge.swellLower;
                swellInner = bulge.swellInner;
                swellOuter = bulge.swellOuter;
                rigidity = bulge.rigidity;
            }
        }
        public Vector3 GetSoftTorso(float yPerc, Vector2 sin, AnimationCurve ellipseX, AnimationCurve ellipseZ, AnimationCurve ellipseNegZ, out Vector3 softDisplacement)
        {
            float x = ellipseX.Evaluate(yPerc);
            float xDeriv = Derivative(ellipseX, yPerc);

            float z;
            float zDeriv;
            if (sin.y >= 0f)
            {
                z = ellipseZ.Evaluate(yPerc);
                zDeriv = Derivative(ellipseZ, yPerc);
            }
            else
            {
                z = ellipseNegZ.Evaluate(yPerc);
                zDeriv = Derivative(ellipseNegZ, yPerc);
            }

            Vector3 torsoCoords = new Vector3(x * sin.x, 0f, z * sin.y);


            Vector3 breastDisplacement = bulgeBreast.ComputeBulge(yPerc, sin, 0f, UnderbustY, false);
            torsoCoords += breastDisplacement * bulgeBreast.rigidity;
            softDisplacement = breastDisplacement * (1f - bulgeBreast.rigidity);

            Vector3 upperBackDisplacement = bulgeUpperBack.ComputeBulge(yPerc, sin, 0f, UnderbustY, true);
            torsoCoords += upperBackDisplacement * bulgeUpperBack.rigidity;
            softDisplacement += upperBackDisplacement * (1f - bulgeUpperBack.rigidity);

            Vector3 abdomenDisplacement = bulgeAbdomen.ComputeBulge(yPerc, sin, UnderbustY, HighHipsY, false);
            torsoCoords += abdomenDisplacement * bulgeAbdomen.rigidity;
            softDisplacement += abdomenDisplacement * (1f - bulgeAbdomen.rigidity);

            Vector3 lowBackDisplacement = bulgeLowerBack.ComputeBulge(yPerc, sin, UnderbustY, HighHipsY, true);
            torsoCoords += lowBackDisplacement * bulgeLowerBack.rigidity;
            softDisplacement += lowBackDisplacement * (1f - bulgeLowerBack.rigidity);

            Vector3 groinDisplacement = bulgeGroin.ComputeBulge(yPerc, sin, HighHipsY, 2f, false);
            torsoCoords += groinDisplacement * bulgeGroin.rigidity;
            softDisplacement += groinDisplacement * (1f - bulgeGroin.rigidity);

            Vector3 buttDisplacement = bulgeButt.ComputeBulge(yPerc, sin, HighHipsY, 2f, true);
            torsoCoords += buttDisplacement * bulgeButt.rigidity;
            softDisplacement += buttDisplacement * (1f - bulgeButt.rigidity);

            return torsoCoords;
        }
        public AnimationCurve GenerateSpineCurve(float eyeHeight, Vector3 t7Local, Vector3 l1Local, Vector3 l3Local, Vector3 sacrumLocal, Vector3 sternumOffsetPerc, Vector3 hipsOffsetPerc, float t1HeightPerc)
        {
            float neckT = Mathf.Lerp(0f, -1f, -sternumOffsetPerc.y / (1f - (t1HeightPerc + sternumOffsetPerc.y)));
            neckT = Mathf.Min(neckT, -.15f);

            float jawT = Mathf.Lerp(-1f, 0f, _chinY * .6f / (1f - (t1HeightPerc + sternumOffsetPerc.y)));
            jawT = Mathf.Min(jawT, neckT - .05f);


            float divByEyeHeight = 1f / eyeHeight;
            Vector3 t7Perc = t7Local * divByEyeHeight;
            Vector3 l1Perc = l1Local * divByEyeHeight;
            Vector3 l3Perc = l3Local * divByEyeHeight;
            Vector3 sacrumPerc = sacrumLocal * divByEyeHeight;
            Vector3 t7PercLessSacrum = -sternumOffsetPerc + t7Perc;
            Vector3 totalThorLumbar = t7Perc + l1Perc + l3Perc + sacrumPerc;
            Vector3 totalSternumToHip = -sternumOffsetPerc + totalThorLumbar + hipsOffsetPerc;

            float upperChestT = t7PercLessSacrum.y / totalSternumToHip.y;
            Vector3 upperChestOffset = t7PercLessSacrum - (totalSternumToHip * upperChestT);

            float chestT = (t7PercLessSacrum.y + l1Perc.y) / totalSternumToHip.y;
            Vector3 chestOffset = t7PercLessSacrum + l1Perc - (totalSternumToHip * chestT);

            float spineT = (t7PercLessSacrum.y + l1Perc.y + l3Perc.y) / totalSternumToHip.y;
            Vector3 spineOffset = t7PercLessSacrum + l1Perc + l3Perc - (totalSternumToHip * spineT);

            _t7Y = upperChestT;
            _t7OffsetZ = upperChestOffset.z;
            _l1Y = chestT;
            _l1OffsetZ = chestOffset.z;
            _l3Y = spineT;
            _l3OffsetZ = spineOffset.z;


            float underbustZOffset = (_chestEllipseZ - _chestEllipseNegZ) * .2f;
            float waistZOffset = (_waistEllipseZ - _waistEllipseNegZ) * .4f;
            float highHipsZOffset = (_highHipsEllipseZ - _highHipsEllipseNegZ) * .4f;
            AnimationCurve spine = new AnimationCurve(new Keyframe(-2f, 0f, 0f, 0f, 0f, 0f),
                new Keyframe(-1.4f, 0f, 0f, 0f, 0f, 0f),
                new Keyframe(jawT, 0f, 0f, 0f, 0f, 0f),
                new Keyframe(neckT, 0f, 0f, 0f, 0f, 0f),
                new Keyframe(0f, 0f, 0f, 0f, 0f, 0f),
                new Keyframe(_underbustY, underbustZOffset, 0f, 0f, 0f, 0f),
                new Keyframe(_waistY, waistZOffset, 0f, 0f, 0f, 0f),
                new Keyframe(_highHipY, highHipsZOffset, 0f, 0f, 0f, 0f),
                new Keyframe(1f, 0f, 0f, 0f, 0f, 0f),
                new Keyframe(2f, 0f, 0f, 0f, 0f, 0f));



            return spine;
        }
        public AnimationCurve GenerateTorsoZCurve(Vector3 sternumOffsetPerc, float t1HeightPerc, AnimationCurve spineZ)
        {
            (float neckT, float jawT) = GetNeckAndJawT(sternumOffsetPerc, t1HeightPerc);
            var ellipseZ = new AnimationCurve(new Keyframe(-2f, 0f - spineZ.Evaluate(-2f), _headEllipseZ * 3.141f, _headEllipseZ * 3.141f),
                new Keyframe(-1.4f, _headEllipseZ - spineZ.Evaluate(-1.4f), 0f, 0f),
                new Keyframe(jawT, _jawEllipseZ - spineZ.Evaluate(jawT), 0f, 0f),
                new Keyframe(neckT, _neckEllipseZ - spineZ.Evaluate(neckT), 0f, 0f),
                new Keyframe(0f, _sternumEllipseZ - spineZ.Evaluate(0f), (_neckEllipseZ - _sternumEllipseZ) / neckT, (_chestEllipseZ - _sternumEllipseZ) / _underbustY),
                new Keyframe(_underbustY, _chestEllipseZ - spineZ.Evaluate(_underbustY), 0f, 0f),
                new Keyframe(_waistY, _waistEllipseZ - spineZ.Evaluate(_waistY), 0f, 0f),
                new Keyframe(_highHipY, _highHipsEllipseZ - spineZ.Evaluate(_highHipY), 0f, 0f),
                new Keyframe(1f, _hipsEllipseZ - spineZ.Evaluate(1f), (_hipsEllipseZ - _highHipsEllipseZ) * (1f - _highHipY) * 4f, (_hipsEllipseZ - _highHipsEllipseZ) * (2f - 1f)),
                new Keyframe(2f, 0f - spineZ.Evaluate(2f), _hipsEllipseZ * -3.141f, _hipsEllipseZ * -3.141f));
            return ellipseZ;
        }
        public AnimationCurve GenerateTorsoNegZCurve(Vector3 sternumOffsetPerc, float t1HeightPerc, AnimationCurve spineZ)
        {
            (float neckT, float jawT) = GetNeckAndJawT(sternumOffsetPerc, t1HeightPerc);
            var ellipseNegZ = new AnimationCurve(new Keyframe(-2f, 0f + spineZ.Evaluate(-2f), _headEllipseNegZ * 3.141f, _headEllipseNegZ * 3.141f),
                new Keyframe(-1.4f, _headEllipseNegZ + spineZ.Evaluate(-1.4f), 0f, 0f),
                new Keyframe(jawT, _jawEllipseNegZ + spineZ.Evaluate(jawT), 0f, 0f),
                new Keyframe(neckT, _neckEllipseNegZ + spineZ.Evaluate(neckT), 0f, 0f),
                new Keyframe(0f, _sternumEllipseNegZ + spineZ.Evaluate(0f), 0f, 0f),
                new Keyframe(_underbustY, _chestEllipseNegZ + spineZ.Evaluate(_underbustY), 0f, 0f),
                new Keyframe(_waistY, _waistEllipseNegZ + spineZ.Evaluate(_waistY), 0f, 0f),
                new Keyframe(_highHipY, _highHipsEllipseNegZ + spineZ.Evaluate(_highHipY), 0f, 0f),
                new Keyframe(1f, _hipsEllipseNegZ + spineZ.Evaluate(1f), 0f, 0f),
                new Keyframe(2f, 0f + spineZ.Evaluate(2f), _hipsEllipseNegZ * -3.141f, _hipsEllipseNegZ * -3.141f));
            return ellipseNegZ;
        }
        public AnimationCurve GenerateTorsoXCurve(float sternumEllipseX, float hipEllipseX, Vector3 sternumOffsetPerc, float t1HeightPerc)
        {
            (float neckT, float jawT) = GetNeckAndJawT(sternumOffsetPerc, t1HeightPerc);
            var ellipseX = new AnimationCurve(new Keyframe(-2f, 0f, _headEllipseX * 3.141f, _headEllipseX * 3.141f),
                new Keyframe(-1.4f, _headEllipseX, 0f, 0f),
                new Keyframe(jawT, _jawEllipseX, 0f, 0f),
                new Keyframe(neckT, _neckEllipseX, 0f, 0f),
                new Keyframe(0f, sternumEllipseX, 0f, 0f),
                new Keyframe(_underbustY, _chestEllipseX, 0f, 0f),
                new Keyframe(_waistY, _waistEllipseX, 0f, 0f),
                new Keyframe(_highHipY, _highHipsEllipseX, 0f, 0f),
                new Keyframe(1f, _hipsEllipseX, 0f, 0f),
                new Keyframe(2f, hipEllipseX, (_hipsEllipseX - hipEllipseX) * -3.141f, (_hipsEllipseX - hipEllipseX) * -3.141f));
            return ellipseX;
        }
        private (float, float) GetNeckAndJawT(Vector3 sternumOffsetPerc, float t1HeightPerc)
        {
            float neckT = Mathf.Lerp(0f, -1f, -sternumOffsetPerc.y / (1f - (t1HeightPerc + sternumOffsetPerc.y)));
            neckT = Mathf.Min(neckT, -.15f);

            float jawT = Mathf.Lerp(-1f, 0f, _chinY * .6f / (1f - (t1HeightPerc + sternumOffsetPerc.y)));
            jawT = Mathf.Min(jawT, neckT - .05f);

            return (neckT, jawT);
        }
        public float Derivative(AnimationCurve self, float time)
        {
            if (self == null) return 0.0f;
            for (int i = 0; i < self.length - 1; i++)
            {
                if (time < self[i].time) continue;
                if (time > self[i + 1].time) continue;
                return Derivative(self[i], self[i + 1], (time - self[i].time) / (self[i + 1].time - self[i].time));
            }
            return 0.0f;
        }

        private float Derivative(Keyframe from, Keyframe to, float lerp)
        {
            float dt = to.time - from.time;

            float m0 = from.outTangent * dt;
            float m1 = to.inTangent * dt;

            float lerp2 = lerp * lerp;

            float a = 6.0f * lerp2 - 6.0f * lerp;
            float b = 3.0f * lerp2 - 4.0f * lerp + 1.0f;
            float c = 3.0f * lerp2 - 2.0f * lerp;
            float d = -a;

            return a * from.value + b * m0 + c * m1 + d * to.value;
        }




























    }
}
