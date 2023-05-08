import pandas as pd
import numpy as np
import os

def export_csv(points, output_folder):
    x = points[:, 0]
    y = points[:, 1]
    angle = points[:, 2]
    w, h = points[:, 5], points[:, 6]
    score = points[:, 4] * 100
    centerx = np.int32(x + w/2)
    centery = np.int32(y + h/2)

    centerx_series = pd.Series(centerx, name='x')
    centery_series = pd.Series(centery, name='y')
    angle_series = pd.Series(angle, name='angle')
    score_series = pd.Series(score, name='score')
    index_series = pd.Series(np.arange(len(score)), name='index')

    result = pd.concat([index_series, centerx_series, centery_series, angle_series, score_series], axis=1)

    if not os.path.exists(output_folder):
        os.makedirs(output_folder)
    result.to_csv(os.path.join(output_folder, 'result.csv'), index=False)