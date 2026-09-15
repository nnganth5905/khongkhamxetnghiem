import React, {
  useEffect,
  useState,
} from 'react';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import {
  getAdminDashboardStats,
  getAdminErrorMessage,
  getAdminFiles,
} from '../../services/adminService';

const DEFAULT_STATS = {
  totalCustomers: 0,
  totalEmployees: 0,
  totalTestOrders: 0,
  totalResults: 0,
  testCompletionPercent: 0,
};

const formatFileSize = (
  bytes
) => {
  const size =
    Number(bytes || 0);

  if (size <= 0) {
    return '0 B';
  }

  const units = [
    'B',
    'KB',
    'MB',
    'GB',
    'TB',
  ];

  const index =
    Math.min(
      Math.floor(
        Math.log(size) /
          Math.log(1024)
      ),
      units.length - 1
    );

  const value =
    size /
    Math.pow(
      1024,
      index
    );

  return `${value.toFixed(
    index === 0
      ? 0
      : 2
  )} ${units[index]}`;
};

export default function AdminDashboard() {
  const [stats, setStats] =
    useState(
      DEFAULT_STATS
    );

  const [
    statsLoading,
    setStatsLoading,
  ] = useState(true);

  const [
    filesLoading,
    setFilesLoading,
  ] = useState(true);

  const [error, setError] =
    useState('');

  const [
    currentPath,
    setCurrentPath,
  ] = useState('');

  const [
    parentPath,
    setParentPath,
  ] = useState(null);

  const [
    directories,
    setDirectories,
  ] = useState([]);

  const [files, setFiles] =
    useState([]);

  const loadStats =
    async () => {
      try {
        setStatsLoading(
          true
        );

        const data =
          await getAdminDashboardStats();

        setStats({
          totalCustomers:
            Number(
              data?.totalCustomers ??
                data?.tongKhachHang ??
                0
            ),

          totalEmployees:
            Number(
              data?.totalEmployees ??
                data?.tongNhanVien ??
                0
            ),

          totalTestOrders:
            Number(
              data?.totalTestOrders ??
                data?.tongPhieuXetNghiem ??
                0
            ),

          totalResults:
            Number(
              data?.totalResults ??
                data?.tongKetQua ??
                0
            ),

          testCompletionPercent:
            Number(
              data?.testCompletionPercent ??
                data?.tiLeHoanThanh ??
                0
            ),
        });
      } catch (err) {
        setError(
          getAdminErrorMessage(
            err,
            'Không thể tải thống kê.'
          )
        );
      } finally {
        setStatsLoading(
          false
        );
      }
    };

  const loadFiles =
    async (
      path = ''
    ) => {
      try {
        setFilesLoading(
          true
        );

        const data =
          await getAdminFiles(
            path
          );

        setCurrentPath(
          data?.currentPath ??
            path ??
            ''
        );

        setParentPath(
          data?.parentPath ??
            null
        );

        setDirectories(
          data?.directories ??
            []
        );

        setFiles(
          data?.files ?? []
        );
      } catch (err) {
        setError(
          getAdminErrorMessage(
            err,
            'Không thể tải danh sách file.'
          )
        );

        setDirectories(
          []
        );

        setFiles([]);
      } finally {
        setFilesLoading(
          false
        );
      }
    };

  useEffect(() => {
    loadStats();
    loadFiles('');
  }, []);

  const completion =
    Math.max(
      0,
      Math.min(
        100,
        Number(
          stats.testCompletionPercent ||
            0
        )
      )
    );

  return (
    <div>
      <h1 className="dashboard-page-title">
        Báo cáo thống kê
      </h1>

      {error && (
        <Notification
          type="danger"
          message={error}
          onClose={() =>
            setError('')
          }
        />
      )}

      {statsLoading ? (
        <Loading text="Đang tải thống kê..." />
      ) : (
        <div className="dashboard-stats-row">
          {/* KHÁCH HÀNG */}

          <div className="dashboard-stat-card">
            <div className="dashboard-stat-card-header">
              <div className="dashboard-stat-card-title">
                Tổng khách hàng
              </div>

              <div className="dashboard-stat-card-icon">
                <i className="fa-solid fa-users-line" />
              </div>
            </div>

            <div className="dashboard-stat-card-value">
              {
                stats.totalCustomers
              }
            </div>
          </div>

          {/* NHÂN VIÊN */}

          <div className="dashboard-stat-card">
            <div className="dashboard-stat-card-header">
              <div className="dashboard-stat-card-title">
                Tổng nhân viên
              </div>

              <div className="dashboard-stat-card-icon">
                <i className="fa-solid fa-users-gear" />
              </div>
            </div>

            <div className="dashboard-stat-card-value">
              {
                stats.totalEmployees
              }
            </div>
          </div>

          {/* PHIẾU */}

          <div className="dashboard-stat-card">
            <div className="dashboard-stat-card-header">
              <div className="dashboard-stat-card-title">
                Tổng phiếu xét
                nghiệm
              </div>

              <div className="dashboard-stat-card-icon">
                <i className="fa-solid fa-vial-virus" />
              </div>
            </div>

            <div className="dashboard-stat-card-value">
              {
                stats.totalTestOrders
              }

              <div className="dashboard-progress-bar">
                <div
                  className="dashboard-progress-fill"
                  style={{
                    width:
                      `${completion}%`,
                  }}
                />
              </div>

              <div className="small text-secondary mt-2 fw-normal">
                {completion}% hoàn
                thành
              </div>
            </div>
          </div>

          {/* KẾT QUẢ */}

          <div className="dashboard-stat-card">
            <div className="dashboard-stat-card-header">
              <div className="dashboard-stat-card-title">
                Tổng kết quả
              </div>

              <div className="dashboard-stat-card-icon">
                <i className="fa-solid fa-square-poll-horizontal" />
              </div>
            </div>

            <div className="dashboard-stat-card-value">
              {
                stats.totalResults
              }
            </div>
          </div>
        </div>
      )}

      {/* ==================================================
          FILE MANAGER
      ================================================== */}

      <div className="file-manager-section">
        <div className="d-flex flex-wrap justify-content-between align-items-center gap-2">
          <h2 className="mb-0">
            Quản lý file
          </h2>

          <button
            type="button"
            className="btn btn-sm btn-outline-primary"
            onClick={() =>
              loadFiles(
                currentPath
              )
            }
            disabled={
              filesLoading
            }
          >
            <i className="fa-solid fa-rotate me-2" />

            Làm mới
          </button>
        </div>

        <div className="current-path">
          <strong>
            Đường dẫn hiện tại:
          </strong>{' '}

          {currentPath ||
            '/'}
        </div>

        {parentPath !==
          null && (
          <button
            type="button"
            className="btn btn-outline-secondary btn-sm mb-3"
            onClick={() =>
              loadFiles(
                parentPath
              )
            }
          >
            <i className="fa-solid fa-arrow-left me-2" />

            Thư mục trước
          </button>
        )}

        {filesLoading ? (
          <Loading text="Đang tải file..." />
        ) : (
          <div className="file-manager">
            {/* FOLDERS */}

            <div className="file-manager-group">
              <h3>
                Thư mục
              </h3>

              <div className="file-items-grid">
                {directories.length >
                0 ? (
                  directories.map(
                    (
                      dir,
                      index
                    ) => (
                      <button
                        type="button"
                        key={
                          dir.path ??
                          index
                        }
                        className="file-item directory border"
                        onClick={() =>
                          loadFiles(
                            dir.path
                          )
                        }
                      >
                        <div className="file-item-icon">
                          <i className="fas fa-folder" />
                        </div>

                        <div className="file-item-name">
                          {dir.name}
                        </div>
                      </button>
                    )
                  )
                ) : (
                  <p className="text-secondary small">
                    Không có thư
                    mục nào
                  </p>
                )}
              </div>
            </div>

            {/* FILES */}

            <div className="file-manager-group">
              <h3>
                File
              </h3>

              <div className="file-items-grid">
                {files.length >
                0 ? (
                  files.map(
                    (
                      file,
                      index
                    ) => (
                      <div
                        key={
                          file.path ??
                          index
                        }
                        className="file-item file"
                      >
                        <div className="file-item-icon">
                          <i className="fas fa-file" />
                        </div>

                        <div className="file-item-name">
                          {
                            file.name
                          }
                        </div>

                        <div className="file-item-size">
                          {formatFileSize(
                            file.size
                          )}
                        </div>
                      </div>
                    )
                  )
                ) : (
                  <p className="text-secondary small">
                    Không có file
                    nào
                  </p>
                )}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}