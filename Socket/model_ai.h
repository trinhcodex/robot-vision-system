#include <stdio.h>
#include <math.h>
#include <stdlib.h>

// mô phỏng việc tính toán giá trị output của hệ thống camera

double *compute(double x, double y) {
    static double result[7];

    for (int i = 0; i < 7; i++) {
        result[i] = x + y;
    }
    return result;
}
