import React, {
  useEffect,
  useState,
} from 'react';

import {
  Link,
  useParams,
} from 'react-router-dom';

import Loading from '../../components/Loading';
import Notification from '../../components/Notification';

import { getSocialPost } from '../../services/newsService';
import { getApiErrorMessage } from '../../services/api';

export default function SocialArticle() {
  const { id } = useParams();

  const queryId =
    new URLSearchParams(
      window.location.search
    ).get('id');

  const articleId =
    id || queryId;

  const [article, setArticle] = useState(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let active = true;

    const loadArticle = async () => {
      if (!articleId) {
        setError(
          'Thiếu mã bài viết.'
        );

        setLoading(false);
        return;
      }

      try {
        const data =
          await getSocialPost(
            articleId
          );

        if (active) {
          setArticle(data);
        }
      } catch (err) {
        if (active) {
          setError(
            getApiErrorMessage(
              err,
              'Không tìm thấy bài viết.'
            )
          );
        }
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
  }, [articleId]);

  if (loading) {
    return (
      <Loading text="Đang tải bài viết..." />
    );
  }

  const title =
    article?.title ??
    article?.tieuDe ??
    article?.TieuDe;

  const image =
    article?.image ??
    article?.hinhAnh ??
    article?.HinhAnh;

  const createdAt =
    article?.createdAt ??
    article?.ngayDang ??
    article?.NgayDang ??
    '';

  const author =
    article?.author ??
    article?.tacGia ??
    article?.TacGia ??
    'Bio Medic Center';

  const content =
    article?.content ??
    article?.noiDung ??
    article?.NoiDung ??
    '';

  return (
    <div className="bg-light min-vh-100 py-5">
      <div
        className="container"
        style={{
          maxWidth: 900,
        }}
      >
        {error && (
          <Notification
            type="danger"
            message={error}
          />
        )}

        {article && (
          <article className="card border-0 shadow-sm rounded-4 overflow-hidden">
            {image && (
              <img
                src={image}
                alt={title}
                className="w-100"
                style={{
                  maxHeight: 480,
                  objectFit: 'cover',
                }}
              />
            )}

            <div className="p-4 p-lg-5">
              <h1
                className="fw-bold mb-3"
                style={{
                  color: '#0b3b5f',
                }}
              >
                {title}
              </h1>

              <div className="d-flex flex-wrap gap-3 small text-secondary mb-4">
                {createdAt && (
                  <span>
                    <i className="fa-regular fa-calendar me-2" />
                    {createdAt}
                  </span>
                )}

                <span>
                  <i className="fa-regular fa-user me-2" />
                  {author}
                </span>
              </div>

              <div
                className="article-content"
                style={{
                  whiteSpace: 'pre-line',
                  lineHeight: 1.9,
                  fontSize: '17px',
                  color: '#333',
                }}
              >
                {content}
              </div>

              <div className="mt-5 pt-4 border-top">
                <Link
                  to="/hoat-dong-xa-hoi"
                  className="btn btn-outline-primary"
                >
                  <i className="fa-solid fa-arrow-left me-2" />
                  Quay lại hoạt động xã hội
                </Link>
              </div>
            </div>
          </article>
        )}
      </div>
    </div>
  );
}