import React, {
  useEffect,
  useState,
} from 'react';

import {
  Link,
  useParams,
} from 'react-router-dom';

import {
  getMedicalArticle,
  getMedicalErrorMessage,
} from '../../services/medicalService';

const formatDate = (dateStr) => {
  if (!dateStr) {
    return '—';
  }

  const d =
    new Date(dateStr);

  if (
    Number.isNaN(
      d.getTime()
    )
  ) {
    return dateStr;
  }

  const day =
    String(
      d.getDate()
    ).padStart(2, '0');

  const month =
    String(
      d.getMonth() + 1
    ).padStart(2, '0');

  const year =
    d.getFullYear();

  return `${day}/${month}/${year}`;
};

const safeImage = (src) =>
  src ||
  'https://placehold.co/400x300/0B63E5/FFFFFF/png?text=Y+Khoa';

const normalizeArticle = (
  data = {}
) => ({
  id:
    data.id ??
    data.idBaiViet ??
    data.IDBaiViet,

  title:
    data.title ??
    data.tieuDe ??
    data.TieuDe ??
    '',

  content:
    data.content ??
    data.noiDung ??
    data.NoiDung ??
    '',

  image:
    data.image ??
    data.hinhAnh ??
    data.HinhAnh ??
    '',

  createdAt:
    data.createdAt ??
    data.created_at ??
    data.ngayDang ??
    data.NgayDang ??
    '',
});

export default function MedicalArticle() {
  const {
    id: routeId,
  } = useParams();

  const queryId =
    new URLSearchParams(
      window.location.search
    ).get('id');

  const id =
    routeId ||
    queryId;

  const [article, setArticle] =
    useState(null);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState('');

  useEffect(() => {
    let active = true;

    if (
      !id ||
      Number(id) <= 0
    ) {
      setError(
        'Không tìm thấy bài viết.'
      );

      setLoading(false);

      return;
    }

    const loadArticle =
      async () => {
        try {
          setLoading(true);
          setError('');

          const data =
            await getMedicalArticle(
              id
            );

          if (!active) {
            return;
          }

          setArticle(
            normalizeArticle(
              data
            )
          );
        } catch (err) {
          if (!active) {
            return;
          }

          setArticle(null);

          setError(
            getMedicalErrorMessage(
              err,
              'Không tìm thấy bài viết.'
            )
          );
        } finally {
          if (active) {
            setLoading(false);
          }
        }
      };

    loadArticle();

    return () => {
      active = false;
    };
  }, [id]);

  if (loading) {
    return (
      <div className="container py-5 text-center text-secondary">
        <div
          className="spinner-border text-primary mb-2"
          role="status"
        />

        <div>
          Đang tải nội dung bài
          viết...
        </div>
      </div>
    );
  }

  return (
    <div
      className="container my-5"
      style={{
        maxWidth:
          '800px',

        padding:
          '0 15px',
      }}
    >
      {article ? (
        <article className="card border-0 shadow-sm p-4 rounded-3">
          <h1
            className="fw-bold mb-3 text-center"
            style={{
              fontSize:
                '32px',

              color:
                '#0b3b5f',
            }}
          >
            {article.title}
          </h1>

          <div className="text-muted text-center small mb-4">
            <i className="fa-regular fa-calendar me-1" />

            {formatDate(
              article.createdAt
            )}
          </div>

          {article.image && (
            <img
              src={safeImage(
                article.image
              )}
              alt={
                article.title
              }
              className="img-fluid rounded-3 mb-4 mx-auto d-block"
              style={{
                maxHeight:
                  '460px',

                objectFit:
                  'cover',
              }}
            />
          )}

          <div
            className="article-content text-start"
            style={{
              fontSize:
                '17px',

              lineHeight:
                '1.8',

              color:
                '#333',

              whiteSpace:
                'pre-line',
            }}
          >
            {article.content}
          </div>

          <div className="text-center mt-4 pt-3 border-top">
            <Link
              to="/medical"
              className="btn btn-outline-primary px-4"
            >
              &laquo; Quay lại
              danh sách
            </Link>
          </div>
        </article>
      ) : (
        <div>
          <div className="alert alert-danger text-center shadow-sm">
            {error ||
              'Không tìm thấy bài viết.'}
          </div>

          <div className="text-center mt-3">
            <Link
              to="/medical"
              className="btn btn-outline-primary"
            >
              &laquo; Quay lại
              danh sách
            </Link>
          </div>
        </div>
      )}
    </div>
  );
}