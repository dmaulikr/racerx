using UnityEngine;

struct IntVector3 {
    public int x, y, z;

    public float dst(ref IntVector3 o) {
        return Mathf.Sqrt((x - o.x) * (x - o.x) + (y - o.y) * (y - o.y) + (z - o.z) * (z - o.z));
    }

    public override int GetHashCode() {
        unchecked {
            int result = 17;

            result = result * 31 + x.GetHashCode();
            result = result * 31 + y.GetHashCode();
            result = result * 31 + z.GetHashCode();

            return result;
        }
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }

        IntVector3 vec = (IntVector3)obj;
        return vec.x == x && vec.y == y && vec.z == z;
    }
}
